using System.ComponentModel.DataAnnotations;

namespace StarterTemplate.Domain.Entities
{
    /// <summary>
    /// Represents a role entity in the system.
    /// A role defines a set of permissions that can be assigned to users.
    /// </summary>
    public class Role : BaseEntity
    {
        /// <summary>
        /// Gets or sets the unique name of the role.
        /// This is a required field with a maximum length of 50 characters.
        /// </summary>
        [Required]
        [StringLength(50)]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the description of the role.
        /// This field is optional and provides information about what the role is for.
        /// </summary>
        [StringLength(200)]
        public string? Description { get; set; }

        /// <summary>
        /// Gets or sets the navigation property to the users who have this role.
        /// </summary>
        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();

        /// <summary>
        /// Gets or sets the navigation property to the permissions assigned to this role.
        /// </summary>
        public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
    }
}

