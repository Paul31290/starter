namespace StarterTemplate.Application.DTOs
{
    /// <summary>
    /// Data Transfer Object for RolePermission entity.
    /// </summary>
    public class RolePermissionDto : BaseDto
    {
        /// <summary>
        /// Gets or sets the ID of the role.
        /// </summary>
        public int RoleId { get; set; }

        /// <summary>
        /// Gets or sets the role name.
        /// </summary>
        public string? RoleName { get; set; }

        /// <summary>
        /// Gets or sets the ID of the permission.
        /// </summary>
        public int PermissionId { get; set; }

        /// <summary>
        /// Gets or sets the permission details.
        /// </summary>
        public PermissionDto? Permission { get; set; }
    }
}

