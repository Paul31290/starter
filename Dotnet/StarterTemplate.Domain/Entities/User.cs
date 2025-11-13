using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace StarterTemplate.Domain.Entities
{
    /// <summary>
    /// Represents a user entity in the system.
    /// A user is an authenticated individual who can access the application and perform various operations.
    /// </summary>
    public class User : BaseEntity
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
        /// Gets or sets the hashed password for the user.
        /// This field stores the securely hashed password and is required for authentication.
        /// </summary>
        [Required]
        [StringLength(255)]
        [JsonIgnore]
        public string PasswordHash { get; set; } = string.Empty;

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

        /// <summary>
        /// Gets or sets the date and time when the user account was created (inherited from BaseEntity).
        /// Use the base entity's <see cref="BaseEntity.CreatedAt"/> (DateTimeOffset) instead of a dynamic initializer here.
        /// </summary>
        // ...existing code... (CreatedAt is provided by BaseEntity as DateTimeOffset)

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
        /// Gets or sets the navigation property to the roles assigned to this user.
        /// </summary>
        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    }
}