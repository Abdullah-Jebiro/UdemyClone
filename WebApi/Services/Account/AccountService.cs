
using Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Models.Dtos;
using Models.Dtos.Account;
using Models.Dtos.Email;
using Models.Enums;
using Models.Exceptions;
using Models.Identity;
using Models.ResponseModels;
using Models.Settings;
using Services.File;
using Services.Repos;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Security.Claims;
using System.Text;

namespace Services.Account
{
    public class AccountService : IAccountService
    {

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly JWTSettings _jwtSettings;
        private readonly IEmailService _emailService;
        private readonly ApplicationDbContext _context;
        private readonly IFilesService _filesService;

        public AccountService(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IOptions<JWTSettings> jwtSettings, IEmailService emailService,
             IFilesService filesService,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtSettings = jwtSettings.Value;
            _emailService = emailService;
            _context = context;
            _filesService = filesService;

        }


        /// <summary>
        /// Generates a random code string consisting of 7 digits.
        /// </summary>
        /// <returns>The generated code string.</returns>
        private string GenerateRandomCode()
        {
            Random random = new Random();
            string code = random.Next(1000000, 9999999).ToString();
            return code;
        }


        private async Task<JwtSecurityToken> GenerateJwtToken(ApplicationUser user)
        {
            // Get user claims and roles
            var userClaims = await _userManager.GetClaimsAsync(user);
            var roles = await _userManager.GetRolesAsync(user);

            // Create role claims
            var roleClaims = new List<Claim>();
            foreach (var role in roles)
                roleClaims.Add(new Claim(ClaimTypes.Role, role));

            // Combine all claims
            var claims = new[]
            {
              new Claim(ClaimTypes.GivenName, user.UserName),
              new Claim(ClaimTypes.Email, user.Email),
              new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            }
            .Union(userClaims)
            .Union(roleClaims);

            // Generate symmetric security key and signing credentials
            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.DurationInMinutes),
                signingCredentials: signingCredentials);
            return jwtSecurityToken;
        }


        /// <summary>
        /// Sends a verification email to the newly registered user.
        /// </summary>
        /// <param name="user">The newly registered user.</param>
        /// <param name="uri">The base URI of the application.</param>
        /// <returns>The verification URI sent to the user.</returns>
        private async Task<string> SendVerificationEmail(ApplicationUser user, string uri)
        {
            // Generate email confirmation code and encode it
            var emailConfirmationCode = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var encodedEmailConfirmationCode = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(emailConfirmationCode));

            // Create verification URI with user ID and encoded email confirmation code as query string parameters
            var confirmationRoute = "api/account/confirm-email/";
            var endpointUri = new Uri($"{uri}/{confirmationRoute}");
            var verificationUri = QueryHelpers.AddQueryString(endpointUri.ToString(), "userId", user.Id.ToString());
            verificationUri = QueryHelpers.AddQueryString(verificationUri, "code", encodedEmailConfirmationCode);

            // Send verification email to the user
            await _emailService.SendAsync(new EmailRequest
            {
                To = user.Email!,
                Body = $"Please confirm your account by visiting this <a href='{verificationUri}'>URL</a>.",
                Subject = "Confirm Registration"
            });

            return verificationUri;
        }

        public async Task<BaseResponse<AuthenticationResponse>> AuthenticateAsync(AuthenticationRequest request)
        {
            // Find user by email
            var user = await _userManager.FindByEmailAsync(request.Email.Trim());
            if (user == null)
            {
                throw new ApiException(HttpStatusCode.BadRequest, $"User with email '{request.Email}' not found.");
            }

            // Check if the user's account is confirmed
            if (!user.EmailConfirmed)
            {
                throw new ApiException(HttpStatusCode.BadRequest, $"Account not confirmed for '{request.Email}'.");
            }

            // Authenticate user with provided credentials
            var signInResult = await _signInManager.PasswordSignInAsync(user, request.Password, false, lockoutOnFailure: false);
            if (!signInResult.Succeeded)
            {
                throw new ApiException(HttpStatusCode.BadRequest, $"Invalid credentials for '{request.Email}'.");
            }

            // Generate JWT token
            var jwtSecurityToken = await GenerateJwtToken(user);

            // Create authentication response
            var response = new AuthenticationResponse()
            {
                Id = user.Id.ToString(),
                JWToken = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
                Email = user.Email!,
                UserName = user.UserName!,
                Roles = (await _userManager.GetRolesAsync(user)).ToList(),
                IsVerified = user.EmailConfirmed
            };

            return new BaseResponse<AuthenticationResponse>(response, $"User '{user.UserName}' authenticated successfully.");
        }

        /// <summary>
        /// Registers a new user with the provided details and sends a verification email.
        /// </summary>
        /// <param name="request">The registration request details.</param>
        /// <param name="uri">The base URI of the application.</param>
        /// <returns>A BaseResponse containing the newly registered user ID and a message (User Registered. Please confirm your account).</returns>
        public async Task<BaseResponse<string>> RegisterAsync(RegisterRequest request, string uri)
        {
            // Check if the requested username is already taken
            var existingUsername = await _userManager.FindByNameAsync(request.UserName);
            if (existingUsername != null)
            {
                throw new ApiException(HttpStatusCode.BadRequest, $"Username '{request.UserName}' is already taken.");
            }

            // Check if the requested email is already registered
            var existingEmail = await _userManager.FindByEmailAsync(request.Email);
            if (existingEmail != null)
            {
                throw new ApiException(HttpStatusCode.BadRequest, $"Email {request.Email} is already registered.");
            }

            // Create a new ApplicationUser object and save it to the database
            var newUser = new ApplicationUser
            {
                Email = request.Email,
                UserName = request.UserName
            };
            var result = await _userManager.CreateAsync(newUser, request.Password);
            if (result.Succeeded)
            {
                // Assign the Basic role to the new user
                await _userManager.AddToRoleAsync(newUser, Roles.Basic.ToString());

                // Send a verification email to the new user
                var verificationUri = await SendVerificationEmail(newUser, uri);

                return new BaseResponse<string>(newUser.Id.ToString(), message: $"User Registered. Please confirm your account");
            }
            else
            {
                // If the user creation fails, throw an ApiException with the error messages
                throw new ApiException(HttpStatusCode.InternalServerError, $"{string.Join("\n", result.Errors.Select(e => e.Description).ToList())}");
            }
        }

        /// <summary>
        /// Confirms a user's email address using the provided user ID and verification code.
        /// </summary>
        /// <param name="userId">The ID of the user whose email is being confirmed.</param>
        /// <param name="code">The verification code provided in the confirmation email.</param>
        /// <returns>A BaseResponse containing the confirmed user ID and a message indicating successful confirmation.</returns>
        public async Task<BaseResponse<string>> ConfirmEmailAsync(string userId, string code)
        {
            // Find the user by their ID
            var user = await _userManager.FindByIdAsync(userId);

            // Decode the verification code
            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));

            // Confirm the user's email address using the verification code
            var result = await _userManager.ConfirmEmailAsync(user, code);

            // If the email confirmation is successful, return a success message with the user ID
            if (result.Succeeded)
            {
                return new BaseResponse<string>(
                    user.Id.ToString(),
                    message: $"Account Confirmed for {user.Email}. " +
                    $"You can now use the /api/Account/authenticate endpoint.");
            }
            // If the email confirmation fails, throw an ApiException with an error message
            else
            {
                throw new ApiException(
                    HttpStatusCode.InternalServerError, $"An error occured while confirming {user.Email}.");
            }
        }

        /// <summary>
        /// Handles a request to reset a user's password. Generates a random code and sets it as the user's reset password. Sends an email to the user with the reset password code.
        /// </summary>
        /// <param name="request">The password reset request details.</param>
        /// <param name="uri">The base URI of the application.</param>
        /// <returns>A BaseResponse with a message informing the user to check their email to reset their password.</returns>
        public async Task<BaseResponse<string>> ForgotPasswordAsync(ForgotPasswordRequest request, string uri)
        {
            // Check if the user with the given email exists in the system
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
                throw new ApiException(HttpStatusCode.BadRequest, $"No user found with email address '{request.Email}'.");

            // Generate a random code and set it as the user's reset password
            string resetPasswordCode = GenerateRandomCode();
            user.ResetPassword = resetPasswordCode;
            user.ResetPasswordExpiry = DateTime.UtcNow.AddHours(1);
            await _context.SaveChangesAsync();

            // Send an email to the user with the reset password code
            var emailRequest = new EmailRequest()
            {
                Body = $"Hi {request.Email},\r\n\r\nWe've received your request for a single-use code to use with your account." +
                $"\r\n\r\nYour single use code is: {resetPasswordCode}" +
                "\r\n\r\nIf you did not request this code, you can safely ignore this email. " +
                "Someone else may have mistakenly typed your email address.",
                To = request.Email,
                Subject = "Reset Password",
            };
            await _emailService.SendAsync(emailRequest);

            // Return a message informing the user to check their email to reset their password
            return new BaseResponse<string>(data: user.Email!, message: "Please check your email to reset your password.");
        }


        public async Task<List<ApplicationUser>> GetUsersAsync()
        {
            return await _userManager.Users.Include(u => u.UserRoles).ThenInclude(ur => ur.Role).ToListAsync();
        }

        public async Task<BaseResponse<string>> ResetPasswordAsync(ResetPasswordRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
                throw new ApiException(HttpStatusCode.BadRequest, $"You are not registered with '{request.Email}'.");

            if (user.ResetPassword == request.code && user.ResetPasswordExpiry > DateTime.UtcNow)
            {
                string token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var result = await _userManager.ResetPasswordAsync(user, token, request.Password);
                if (result.Succeeded)
                    return new BaseResponse<string>(request.Email, message: $"Password Resetted.");
                else
                    throw new ApiException(HttpStatusCode.BadRequest, $"Error occurred while resetting the password. Please try again.");
            }
            else
            {
                throw new ApiException(HttpStatusCode.BadRequest, $"Error occurred while resetting the password. Please try again.");
            }
        }




        /// <summary>
        /// Updates the user (Instructors) information in the database.
        /// </summary>
        /// <param name="request">he user information to update.</param>
        /// <param name="userId">he user information to update.</param>
        /// <returns> {Promise<BaseResponse<string>>} - A response indicating whether the update was successful or not.</returns>
        /// <exception cref="ApiException"></exception>
        public async Task<BaseResponse<string>> UpdateUserInfoAsync(UserForUpdateDto request, string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            // If the user doesn't exist, throw an exception.
            if (user == null)
                throw new ApiException(HttpStatusCode.BadRequest, $"User with ID {userId} not found.");

            // Update the user information.
            user.About = request.About;
            bool x = !string.IsNullOrEmpty(request.ProfilePictureUrl);
            if (user.ProfilePictureUrl != request.ProfilePictureUrl && !string.IsNullOrEmpty(request.ProfilePictureUrl))
            {
                if (user.ProfilePictureUrl != "default.jpg")
                {
                    await _filesService.DeleteAsync(user.ProfilePictureUrl);
                }
                user.ProfilePictureUrl = request.ProfilePictureUrl;

            }
            // Save the changes to the database.
            var result = await _userManager.UpdateAsync(user);

            // If the update was successful, return a success response.
            if (result.Succeeded)
                return new BaseResponse<string>(user.Email!, message: "User information updated successfully.");

            // If the update failed, throw an exception.
            else
                throw new ApiException(HttpStatusCode.BadRequest, "Failed to update user information. Please try again.");
        }




        /// <summary>
        /// Retrieves a user with a specified user ID from the database.
        /// </summary>
        /// <param name="userId">The ID of the user to retrieve.</param>
        /// <returns>The user object.</returns>
        public async Task<ApplicationUser> GetUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) throw new ApiException(HttpStatusCode.BadRequest, $"Error");
            return user;
        }


        //TODO
        public async Task<BaseResponse<string>> AddRoleForUser(int userId, int roleId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {

            }

            var role = await _context.Roles.FirstOrDefaultAsync(r => r.Id == roleId);
            if (role == null)
            {
                return new BaseResponse<string>("Role not found.");
            }

            var userRoles = await _userManager.GetRolesAsync(user);
            if (!userRoles.Contains(role.Name))
            {
                await _userManager.AddToRoleAsync(user, role.Name);
            }

            return new BaseResponse<string>("Role added successfully.");
        }

        public async Task<BaseResponse<string>> DeleteRoleForUser(int userId, int roleId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                return new BaseResponse<string>("User not found.");
            }

            var role = await _context.Roles.FirstOrDefaultAsync(r => r.Id == roleId);
            if (role == null)
            {
                return new BaseResponse<string>("Role not found.");
            }

            var userRoles = await _userManager.GetRolesAsync(user);
            if (userRoles.Contains(role.Name))
            {
                await _userManager.RemoveFromRoleAsync(user, role.Name);
            }

            return new BaseResponse<string>("Role removed successfully.");
        }


    }
}


