using StarterTemplate.Application.DTOs;

namespace StarterTemplate.Application.Interfaces
{
    /// <summary>
    /// Interface for JWT authentication service operations.
    /// Provides methods for JWT token generation, validation, and authentication.
    /// </summary>
    public interface IJwtAuthService
    {
        /// <summary>
        /// Authenticates a user with username/email and password.
        /// </summary>
        /// <param name="loginDto">The login credentials.</param>
        /// <returns>Authentication response with tokens and user info if successful; otherwise, null.</returns>
        Task<AuthResponseDto?> LoginAsync(LoginDto loginDto);

        /// <summary>
        /// Registers a new user account.
        /// </summary>
        /// <param name="registerDto">The registration information.</param>
        /// <returns>Authentication response with tokens and user info if successful; otherwise, null.</returns>
        Task<AuthResponseDto?> RegisterAsync(RegisterDto registerDto);

        /// <summary>
        /// Refreshes an access token using a refresh token.
        /// </summary>
        /// <param name="refreshTokenDto">The refresh token information.</param>
        /// <returns>New authentication response with tokens if successful; otherwise, null.</returns>
        Task<AuthResponseDto?> RefreshTokenAsync(RefreshTokenDto refreshTokenDto);

        /// <summary>
        /// Revokes a refresh token, effectively logging out the user.
        /// </summary>
        /// <param name="refreshToken">The refresh token to revoke.</param>
        /// <returns>True if the token was revoked successfully; otherwise, false.</returns>
        Task<bool> RevokeRefreshTokenAsync(string refreshToken);

        /// <summary>
        /// Validates a JWT token and returns the user ID if valid.
        /// </summary>
        /// <param name="token">The JWT token to validate.</param>
        /// <returns>The user ID if the token is valid; otherwise, null.</returns>
        Task<int?> ValidateTokenAsync(string token);

        /// <summary>
        /// Generates a new JWT access token for a user.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <param name="userName">The username.</param>
        /// <param name="email">The user's email.</param>
        /// <param name="roles">The user's roles.</param>
        /// <returns>The generated JWT token.</returns>
        string GenerateAccessToken(int userId, string userName, string email, IEnumerable<string> roles);

        /// <summary>
        /// Generates a new refresh token.
        /// </summary>
        /// <returns>The generated refresh token.</returns>
        string GenerateRefreshToken();

        /// <summary>
        /// Validates a refresh token.
        /// </summary>
        /// <param name="refreshToken">The refresh token to validate.</param>
        /// <returns>True if the refresh token is valid; otherwise, false.</returns>
        Task<bool> ValidateRefreshTokenAsync(string refreshToken);

        /// <summary>
        /// Sends a password reset link to the specified email address.
        /// </summary>
        /// <param name="email">The email address of the user requesting the password reset.</param>
        /// <returns>A task that represents the asynchronous operation, with a boolean result indicating success or failure.</returns>
        Task<bool> SendPasswordResetLinkAsync(string email);
        
        /// <summary>
        /// Resets the user's password using the provided reset token and new password.
        /// </summary>
        /// <param name="resetPasswordDto">The DTO containing the reset password information.</param>
        /// <returns>A task that represents the asynchronous operation, with a ValidationResultDto indicating success or failure.</returns>
        Task<ValidationResultDto> ResetPasswordAsync(ResetPasswordDto resetPasswordDto);
    }
}
