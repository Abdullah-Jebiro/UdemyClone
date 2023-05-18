
using System;
using AutoMapper;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Models.Dtos;
using Models.Dtos.Account;
using Services.Account;
using static Org.BouncyCastle.Math.EC.ECCurve;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;

        public AccountController(IAccountService accountService , IMapper mapper, IConfiguration config)
        {
            _accountService = accountService;;
            _mapper = mapper;
            _config = config;
        }

        /// <summary>
        /// Authenticates a user.
        /// </summary>
        /// <param name="request">The authentication request object containing the user's email and password.</param>
        /// <returns>A BaseResponse object containing the authentication token and other relevant data.</returns>
        [HttpPost("authenticate")]
        public async Task<IActionResult> AuthenticateAsync(AuthenticationRequest request)
        {
            var result = await _accountService.AuthenticateAsync(request);
            return Ok(result);
        }

        /// <summary>
        /// Registers a new user with the provided information
        /// </summary>
        /// <param name="request">The registration request data</param>
        /// <returns>A response indicating success (sends a confirmation email) or failure of the registration process</returns>
        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync(RegisterRequest request)
        {
            var callbackUrl = $"{Request.Scheme}://{Request.Host.Value}"; 
            var response = await _accountService.RegisterAsync(request, callbackUrl);
            return Ok(response);
        }

        /// <summary>
        /// Endpoint for confirming user's email address
        /// </summary>
        /// <param name="userId">The user's Id</param>
        /// <param name="code">The confirmation code</param>
        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmEmailAsync(string userId, string code)
        {
            await _accountService.ConfirmEmailAsync(userId, code);
            var url = _config.GetValue<string>("UrlPages:LoginUrl");
            return Redirect(url!);
        }

        /// <summary>
        /// API endpoint to handle user forgot password requests
        /// </summary>
        /// <param name="request">The forgot password request object</param>
        /// <returns>Returns a HTTP response with BaseResponse &lt;string &gt;</returns>
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPasswordAsync(ForgotPasswordRequest request)
        {
            var uri = $"{Request.Scheme}://{Request.Host.Value}";
            var result = await _accountService.ForgotPasswordAsync(request, uri);
            return Ok(result);
        }

    


        /// <summary>
        /// Handles a HTTP POST request to reset a user's password.
        /// </summary>
        /// <param name="resetPasswordRequest">The request containing information about the user whose password needs to be reset.</param>
        /// <returns>An HTTP 200 (OK) response with the result of the password reset operation.</returns>
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPasswordAsync(ResetPasswordRequest resetPasswordRequest)
        {
            var passwordResetResult = await _accountService.ResetPasswordAsync(resetPasswordRequest);
            return Ok(passwordResetResult);
        }


        /// <summary>
        /// Handles a HTTP PUT request to update a user's information.
        /// </summary>
        /// <param name="userForUpdateDto">The request containing the updated information for the user.</param>
        /// <returns>An HTTP 200 (OK) response with the result of the update operation.</returns>
        [Authorize]
        [HttpPut("update-info")]
        public async Task<IActionResult> Update(UserForUpdateDto userForUpdateDto)
        {
            string userId = User.Identity.GetUserId();
            var updateResult = await _accountService.UpdateUserInfoAsync(userForUpdateDto, userId);
            return Ok(updateResult);

        }

        /// <summary>
        /// Handles a HTTP GET request to retrieve a user's information.
        /// </summary>
        /// <returns>An HTTP 200 (OK) response with the user's information.</returns>
        [Authorize]
        [HttpGet("get-info")]
        public async Task<IActionResult> GetUserInfo()
        {

            string userId = User.Identity.GetUserId();
            var user = await _accountService.GetUserAsync(userId);
            var userForUpdateDto = _mapper.Map<UserForUpdateDto>(user);
            return Ok(userForUpdateDto);
        }

    }
}
