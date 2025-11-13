using System.ComponentModel.DataAnnotations;

namespace StarterTemplate.Application.DTOs
{
    /// <summary>
    /// Data Transfer Object for User entities.
    /// Used for transferring user data between the API and client applications.
    /// Note: Password information is not included in this DTO for security reasons.
    /// </summary>
    public class UserDto : BaseDto
    {
        /// <summary>
        /// Gets or sets the unique username for the user.
        /// This is a required field with a maximum length of 50 characters.
        /// </summary>
        [Required]
        [StringLength(50)]
        public string UserName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the email address of the user.
        /// This is a required field with a maximum length of 100 characters and must be a valid email format.
        /// </summary>
        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the first name of the user.
        /// This field is optional.
        /// </summary>
        [StringLength(50)]
        public string? FirstName { get; set; }

        /// <summary>
        /// Gets or sets the last name of the user.
        /// This field is optional.
        /// </summary>
        [StringLength(50)]
        public string? LastName { get; set; }

        // CreatedAt is inherited from BaseDto (DateTimeOffset?)

        /// <summary>
        /// Gets or sets the date and time of the user's last login.
        /// This field is optional and is updated on each successful login.
        /// </summary>
        public DateTime? LastLoginAt { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the user account is active.
        /// Inactive users cannot log in to the system.
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Gets or sets the profile picture of the user as a base64 encoded string.
        /// This field is optional and stores the user's profile image.
        /// </summary>
        [StringLength(1000000)] // Allow for large base64 strings
        public string? ProfilePicture { get; set; }

        /// <summary>
        /// Gets or sets the list of role names assigned to the user.
        /// </summary>
        public List<string> Roles { get; set; } = new List<string>();
    }
}