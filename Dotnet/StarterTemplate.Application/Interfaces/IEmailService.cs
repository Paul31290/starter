using System.Threading.Tasks;

namespace StarterTemplate.Application.Interfaces
{
    /// <summary>
    /// Interface for email service operations.
    /// Provides methods for sending various types of emails.
    /// </summary>
    public interface IEmailService
    {
        /// <summary>
        /// Sends an email asynchronously.
        /// </summary>
        /// <param name="to">Recipient email address.</param>
        /// <param name="subject">Email subject.</param>
        /// <param name="body">Email body content.</param>
        /// <param name="isHtml">Whether the body content is HTML.</param>
        /// <returns>True if email was sent successfully; otherwise, false.</returns>
        Task<bool> SendEmailAsync(string to, string subject, string body, bool isHtml = true);

        /// <summary>
        /// Sends an email to multiple recipients asynchronously.
        /// </summary>
        /// <param name="to">List of recipient email addresses.</param>
        /// <param name="subject">Email subject.</param>
        /// <param name="body">Email body content.</param>
        /// <param name="isHtml">Whether the body content is HTML.</param>
        /// <returns>True if email was sent successfully; otherwise, false.</returns>
        Task<bool> SendEmailAsync(string[] to, string subject, string body, bool isHtml = true);

        /// <summary>
        /// Sends a password reset email asynchronously.
        /// </summary>
        /// <param name="to">Recipient email address.</param>
        /// <param name="resetToken">Password reset token.</param>
        /// <param name="userName">User's name.</param>
        /// <returns>True if email was sent successfully; otherwise, false.</returns>
        Task<bool> SendPasswordResetEmailAsync(string to, string resetToken, string userName);

        /// <summary>
        /// Sends a welcome email asynchronously.
        /// </summary>
        /// <param name="to">Recipient email address.</param>
        /// <param name="userName">User's name.</param>
        /// <param name="temporaryPassword">Temporary password if applicable.</param>
        /// <returns>True if email was sent successfully; otherwise, false.</returns>
        Task<bool> SendWelcomeEmailAsync(string to, string userName, string? temporaryPassword = null);

        /// <summary>
        /// Sends a notification email asynchronously.
        /// </summary>
        /// <param name="to">Recipient email address.</param>
        /// <param name="title">Notification title.</param>
        /// <param name="message">Notification message.</param>
        /// <returns>True if email was sent successfully; otherwise, false.</returns>
        Task<bool> SendNotificationEmailAsync(string to, string title, string message);
    }
}
