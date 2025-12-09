using StarterTemplate.Application.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace StarterTemplate.Application.Services
{
    /// <summary>
    /// Service for handling email operations.
    /// Provides methods for sending various types of emails including notifications, password resets, and welcome emails.
    /// </summary>
    public class EmailService : IEmailService
    {
        private readonly ILogger<EmailService> _logger;
        private readonly EmailSettings _emailSettings;

        public EmailService(ILogger<EmailService> logger, IOptions<EmailSettings> emailSettings)
        {
            _logger = logger;
            _emailSettings = emailSettings.Value;
        }

        /// <summary>
        /// Sends an email asynchronously.
        /// </summary>
        /// <param name="to">Recipient email address.</param>
        /// <param name="subject">Email subject.</param>
        /// <param name="body">Email body content.</param>
        /// <param name="isHtml">Whether the body content is HTML.</param>
        /// <returns>True if email was sent successfully; otherwise, false.</returns>
        public async Task<bool> SendEmailAsync(string to, string subject, string body, bool isHtml = true)
        {
            try
            {
                using var client = new SmtpClient(_emailSettings.SmtpServer, _emailSettings.SmtpPort);
                client.EnableSsl = true;
                client.Credentials = new NetworkCredential(_emailSettings.SmtpUsername, _emailSettings.SmtpPassword);

                using var message = new MailMessage();
                message.From = new MailAddress(_emailSettings.FromEmail, _emailSettings.FromName);
                message.To.Add(to);
                message.Subject = subject;
                message.Body = body;
                message.IsBodyHtml = isHtml;

                await client.SendMailAsync(message);
                
                _logger.LogInformation("Email sent successfully to {Email} with subject: {Subject}", to, subject);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {Email} with subject: {Subject}", to, subject);
                return false;
            }
        }

        /// <summary>
        /// Sends an email to multiple recipients asynchronously.
        /// </summary>
        /// <param name="to">List of recipient email addresses.</param>
        /// <param name="subject">Email subject.</param>
        /// <param name="body">Email body content.</param>
        /// <param name="isHtml">Whether the body content is HTML.</param>
        /// <returns>True if email was sent successfully; otherwise, false.</returns>
        public async Task<bool> SendEmailAsync(string[] to, string subject, string body, bool isHtml = true)
        {
            try
            {
                using var client = new SmtpClient(_emailSettings.SmtpServer, _emailSettings.SmtpPort);
                client.EnableSsl = true;
                client.Credentials = new NetworkCredential(_emailSettings.SmtpUsername, _emailSettings.SmtpPassword);

                using var message = new MailMessage();
                message.From = new MailAddress(_emailSettings.FromEmail, _emailSettings.FromName);
                
                foreach (var email in to)
                {
                    message.To.Add(email);
                }
                
                message.Subject = subject;
                message.Body = body;
                message.IsBodyHtml = isHtml;

                await client.SendMailAsync(message);
                
                _logger.LogInformation("Email sent successfully to {RecipientCount} recipients with subject: {Subject}", to.Length, subject);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {RecipientCount} recipients with subject: {Subject}", to.Length, subject);
                return false;
            }
        }

        /// <summary>
        /// Sends a password reset email asynchronously.
        /// </summary>
        /// <param name="to">Recipient email address.</param>
        /// <param name="resetToken">Password reset token.</param>
        /// <param name="userName">User's name.</param>
        /// <returns>True if email was sent successfully; otherwise, false.</returns>
        public async Task<bool> SendPasswordResetEmailAsync(string to, string resetToken, string userName)
        {
            var subject = "Password Reset Request";
            var resetUrl = $"{_emailSettings.BaseUrl}/auth/reset-password?token={resetToken}";
            
            var body = new StringBuilder();
            body.AppendLine("<html><body>");
            body.AppendLine($"<h2>Password Reset Request</h2>");
            body.AppendLine($"<p>Hello {userName},</p>");
            body.AppendLine("<p>You have requested to reset your password. Click the link below to reset your password:</p>");
            body.AppendLine($"<p><a href=\"{resetUrl}\" style=\"background-color: #007bff; color: white; padding: 10px 20px; text-decoration: none; border-radius: 5px;\">Reset Password</a></p>");
            body.AppendLine($"<p>Or copy and paste this link into your browser: {resetUrl}</p>");
            body.AppendLine("<p>This link will expire in 24 hours.</p>");
            body.AppendLine("<p>If you did not request this password reset, please ignore this email.</p>");
            body.AppendLine("<p>Best regards,<br/>The StarterTemplate Team</p>");
            body.AppendLine("</body></html>");

            return await SendEmailAsync(to, subject, body.ToString(), true);
        }

        /// <summary>
        /// Sends a welcome email asynchronously.
        /// </summary>
        /// <param name="to">Recipient email address.</param>
        /// <param name="userName">User's name.</param>
        /// <param name="temporaryPassword">Temporary password if applicable.</param>
        /// <returns>True if email was sent successfully; otherwise, false.</returns>
        public async Task<bool> SendWelcomeEmailAsync(string to, string userName, string? temporaryPassword = null)
        {
            var subject = "Welcome to StarterTemplate";
            
            var body = new StringBuilder();
            body.AppendLine("<html><body>");
            body.AppendLine($"<h2>Welcome to StarterTemplate!</h2>");
            body.AppendLine($"<p>Hello {userName},</p>");
            body.AppendLine("<p>Welcome to StarterTemplate! Your account has been successfully created.</p>");
            
            if (!string.IsNullOrEmpty(temporaryPassword))
            {
                body.AppendLine("<p>Your temporary password is: <strong>" + temporaryPassword + "</strong></p>");
                body.AppendLine("<p>Please change this password after your first login for security reasons.</p>");
            }
            
            body.AppendLine("<p>You can now log in to your account and start using the application.</p>");
            body.AppendLine("<p>If you have any questions or need assistance, please don't hesitate to contact our support team.</p>");
            body.AppendLine("<p>Best regards,<br/>The StarterTemplate Team</p>");
            body.AppendLine("</body></html>");

            return await SendEmailAsync(to, subject, body.ToString(), true);
        }

        /// <summary>
        /// Sends a notification email asynchronously.
        /// </summary>
        /// <param name="to">Recipient email address.</param>
        /// <param name="title">Notification title.</param>
        /// <param name="message">Notification message.</param>
        /// <returns>True if email was sent successfully; otherwise, false.</returns>
        public async Task<bool> SendNotificationEmailAsync(string to, string title, string message)
        {
            var subject = $"Notification: {title}";
            
            var body = new StringBuilder();
            body.AppendLine("<html><body>");
            body.AppendLine($"<h2>{title}</h2>");
            body.AppendLine($"<p>{message}</p>");
            body.AppendLine("<p>Best regards,<br/>The StarterTemplate Team</p>");
            body.AppendLine("</body></html>");

            return await SendEmailAsync(to, subject, body.ToString(), true);
        }
    }

    /// <summary>
    /// Configuration settings for email service.
    /// </summary>
    public class EmailSettings
    {
        public string SmtpServer { get; set; } = string.Empty;
        public int SmtpPort { get; set; }
        public string SmtpUsername { get; set; } = string.Empty;
        public string SmtpPassword { get; set; } = string.Empty;
        public string FromEmail { get; set; } = string.Empty;
        public string FromName { get; set; } = string.Empty;
        public string BaseUrl { get; set; } = string.Empty;
    }
}
