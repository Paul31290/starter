using StarterTemplate.Application.DTOs;

namespace StarterTemplate.Application.Interfaces
{
    /// <summary>
    /// Interface for security service operations.
    /// Provides methods for security-related operations like password policies and account lockout.
    /// </summary>
    public interface ISecurityService
    {
        /// <summary>
        /// Validates a password against security policies.
        /// </summary>
        /// <param name="password">The password to validate.</param>
        /// <returns>A validation result containing any policy violations.</returns>
        Task<ValidationResultDto> ValidatePasswordAsync(string password);

        /// <summary>
        /// Generates a secure random password.
        /// </summary>
        /// <param name="length">The length of the password to generate.</param>
        /// <param name="includeSpecialChars">Whether to include special characters.</param>
        /// <returns>A secure random password.</returns>
        Task<string> GenerateSecurePasswordAsync(int length = 12, bool includeSpecialChars = true);

        /// <summary>
        /// Checks if an account is locked out.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <returns>True if the account is locked out; otherwise, false.</returns>
        Task<bool> IsAccountLockedOutAsync(int userId);

        /// <summary>
        /// Locks out an account.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="lockoutDuration">The duration of the lockout in minutes.</param>
        /// <param name="reason">The reason for the lockout.</param>
        /// <returns>True if the account was locked out; otherwise, false.</returns>
        Task<bool> LockoutAccountAsync(int userId, int lockoutDuration = 30, string reason = "Security violation");

        /// <summary>
        /// Unlocks an account.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="reason">The reason for unlocking.</param>
        /// <returns>True if the account was unlocked; otherwise, false.</returns>
        Task<bool> UnlockAccountAsync(int userId, string reason = "Manual unlock");

        /// <summary>
        /// Records a failed login attempt.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="ipAddress">The IP address of the attempt.</param>
        /// <param name="userAgent">The user agent of the attempt.</param>
        /// <returns>True if the attempt was recorded; otherwise, false.</returns>
        Task<bool> RecordFailedLoginAttemptAsync(int userId, string? ipAddress = null, string? userAgent = null);

        /// <summary>
        /// Resets failed login attempts for a user.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <returns>True if the attempts were reset; otherwise, false.</returns>
        Task<bool> ResetFailedLoginAttemptsAsync(int userId);

        /// <summary>
        /// Gets the number of failed login attempts for a user.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <returns>The number of failed login attempts.</returns>
        Task<int> GetFailedLoginAttemptsAsync(int userId);

        /// <summary>
        /// Checks if a password has been used recently.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="password">The password to check.</param>
        /// <param name="historyCount">The number of recent passwords to check against.</param>
        /// <returns>True if the password has been used recently; otherwise, false.</returns>
        Task<bool> IsPasswordInHistoryAsync(int userId, string password, int historyCount = 5);

        /// <summary>
        /// Adds a password to the user's password history.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="passwordHash">The hashed password to add to history.</param>
        /// <returns>True if the password was added to history; otherwise, false.</returns>
        Task<bool> AddPasswordToHistoryAsync(int userId, string passwordHash);

        /// <summary>
        /// Validates a two-factor authentication code.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="code">The 2FA code to validate.</param>
        /// <returns>True if the code is valid; otherwise, false.</returns>
        Task<bool> ValidateTwoFactorCodeAsync(int userId, string code);

        /// <summary>
        /// Generates a two-factor authentication code.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <returns>The generated 2FA code.</returns>
        Task<string> GenerateTwoFactorCodeAsync(int userId);

        /// <summary>
        /// Enables two-factor authentication for a user.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <returns>True if 2FA was enabled; otherwise, false.</returns>
        Task<bool> EnableTwoFactorAsync(int userId);

        /// <summary>
        /// Disables two-factor authentication for a user.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <returns>True if 2FA was disabled; otherwise, false.</returns>
        Task<bool> DisableTwoFactorAsync(int userId);

        /// <summary>
        /// Checks if two-factor authentication is enabled for a user.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <returns>True if 2FA is enabled; otherwise, false.</returns>
        Task<bool> IsTwoFactorEnabledAsync(int userId);
    }
}
