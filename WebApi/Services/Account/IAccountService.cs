using Models.Dtos;
using Models.Dtos.Account;
using Models.Identity;
using Models.ResponseModels;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace Services.Account
{
    public interface IAccountService
    {
        /// <summary>
        /// Authenticates a user with their email and password
        /// </summary>
        Task<BaseResponse<AuthenticationResponse>> AuthenticateAsync(AuthenticationRequest request);

        /// <summary>
        /// Registers a new user with the provided information and sends a confirmation email
        /// </summary>
        Task<BaseResponse<string>> RegisterAsync(RegisterRequest request, string confirmationUri);

        /// <summary>
        /// Updates a user's information
        /// </summary>
        Task<BaseResponse<string>> UpdateUserInfoAsync(UserForUpdateDto request, string userId);

        /// <summary>
        /// Gets a user by their ID
        /// </summary>
        Task<ApplicationUser> GetUserAsync(string userId);

        /// <summary>
        /// Confirms a user's email address using a confirmation code
        /// </summary>
        Task<BaseResponse<string>> ConfirmEmailAsync(string userId, string code);

        /// <summary>
        /// Sends a password reset code to a user's email
        /// </summary>
        Task<BaseResponse<string>> ForgotPasswordAsync(ForgotPasswordRequest request, string resetPasswordUri);

        /// <summary>
        /// Resets a user's password using a reset code
        /// </summary>
        Task<BaseResponse<string>> ResetPasswordAsync(ResetPasswordRequest request);

        /// <summary>
        /// Gets a list of all users
        /// </summary>
        Task<List<ApplicationUser>> GetUsersAsync();
    }

}
