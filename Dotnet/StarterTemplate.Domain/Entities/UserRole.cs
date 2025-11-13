using System.ComponentModel.DataAnnotations;

namespace StarterTemplate.Domain.Entities
{
    /// <summary>
    /// Represents a junction entity between users and roles.
    /// This entity implements the many-to-many relationship between users and roles.
    /// </summary>
    public class UserRole : BaseEntity
    {
        /// <summary>
        /// Gets or sets the foreign key reference to the user.
        /// </summary>
        [Required]
        public int UserId { get; set; }

        /// <summary>
        /// Gets or sets the navigation property to the user.
        /// </summary>
        public User? User { get; set; }

        /// <summary>
        /// Gets or sets the foreign key reference to the role.
        /// </summary>
        [Required]
        public int RoleId { get; set; }

        /// <summary>
        /// Gets or sets the navigation property to the role.
        /// </summary>
        public Role? Role { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the role was assigned to the user.
        /// </summary>
        public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
    }
}

