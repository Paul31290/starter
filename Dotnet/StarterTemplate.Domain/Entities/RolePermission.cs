namespace StarterTemplate.Domain.Entities
{
    /// <summary>
    /// Represents the many-to-many relationship between roles and permissions.
    /// This entity links roles with specific permissions.
    /// </summary>
    public class RolePermission : BaseEntity
    {
        /// <summary>
        /// Gets or sets the ID of the role.
        /// </summary>
        public int RoleId { get; set; }

        /// <summary>
        /// Gets or sets the navigation property to the role.
        /// </summary>
        public Role Role { get; set; } = null!;

        /// <summary>
        /// Gets or sets the ID of the permission.
        /// </summary>
        public int PermissionId { get; set; }

        /// <summary>
        /// Gets or sets the navigation property to the permission.
        /// </summary>
        public Permission Permission { get; set; } = null!;
    }
}

