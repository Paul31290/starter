using System.ComponentModel.DataAnnotations;

namespace StarterTemplate.Application.DTOs
{
    /// <summary>
    /// Data Transfer Object for sending emails.
    /// Used for email operations with validation.
    /// </summary>
    public class EmailDto
    {
        /// <summary>
        /// Gets or sets the recipient email address.
        /// </summary>
        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string To { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the email subject.
        /// </summary>
        [Required]
        [StringLength(200)]
        public string Subject { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the email body content.
        /// </summary>
        [Required]
        public string Body { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether the body content is HTML.
        /// </summary>
        public bool IsHtml { get; set; } = true;

        /// <summary>
        /// Gets or sets the CC recipients (comma-separated email addresses).
        /// </summary>
        [StringLength(500)]
        public string? Cc { get; set; }

        /// <summary>
        /// Gets or sets the BCC recipients (comma-separated email addresses).
        /// </summary>
        [StringLength(500)]
        public string? Bcc { get; set; }

        /// <summary>
        /// Gets or sets the priority of the email (High, Normal, Low).
        /// </summary>
        [StringLength(10)]
        public string Priority { get; set; } = "Normal";
    }

    /// <summary>
    /// Data Transfer Object for bulk email operations.
    /// Used for sending emails to multiple recipients.
    /// </summary>
    public class BulkEmailDto
    {
        /// <summary>
        /// Gets or sets the list of recipient email addresses.
        /// </summary>
        [Required]
        [MinLength(1, ErrorMessage = "At least one recipient is required")]
        public List<string> Recipients { get; set; } = new List<string>();

        /// <summary>
        /// Gets or sets the email subject.
        /// </summary>
        [Required]
        [StringLength(200)]
        public string Subject { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the email body content.
        /// </summary>
        [Required]
        public string Body { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether the body content is HTML.
        /// </summary>
        public bool IsHtml { get; set; } = true;

        /// <summary>
        /// Gets or sets the priority of the email (High, Normal, Low).
        /// </summary>
        [StringLength(10)]
        public string Priority { get; set; } = "Normal";
    }

    /// <summary>
    /// Data Transfer Object for email template operations.
    /// Used for sending templated emails.
    /// </summary>
    public class EmailTemplateDto
    {
        /// <summary>
        /// Gets or sets the recipient email address.
        /// </summary>
        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string To { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the email template name.
        /// </summary>
        [Required]
        [StringLength(100)]
        public string TemplateName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the template variables as key-value pairs.
        /// </summary>
        public Dictionary<string, string> Variables { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// Gets or sets the email subject (optional, can be overridden by template).
        /// </summary>
        [StringLength(200)]
        public string? Subject { get; set; }
    }
}
