using System.ComponentModel.DataAnnotations;

namespace StarterTemplate.Domain.Entities
{
    /// <summary>
    /// Represents a permission entity in the system.
    /// A permission defines a specific action or access right that can be assigned to roles.
    /// </summary>
    public class Permission : BaseEntity
    {
        /// <summary>
        /// Gets or sets the unique name/code of the permission (e.g., Product_List_Access, Product_Create, etc.).
        /// This is a required field with a maximum length of 100 characters.
        /// </summary>
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the description of the permission.
        /// This field is optional and provides information about what the permission grants.
        /// </summary>
        [StringLength(200)]
        public string? Description { get; set; }

        /// <summary>
        /// Gets or sets the resource/module this permission applies to (e.g., Products, Dishes, Users).
        /// This helps group related permissions together.
        /// </summary>
        [StringLength(50)]
        public string? Resource { get; set; }

        /// <summary>
        /// Gets or sets the action this permission allows (e.g., List, Create, Update, Delete, View).
        /// </summary>
        [StringLength(50)]
        public string? Action { get; set; }

        /// <summary>
        /// Gets or sets the navigation property to the roles that have this permission.
        /// </summary>
        public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
    }
}

