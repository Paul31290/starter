using System.ComponentModel.DataAnnotations;

namespace StarterTemplate.Application.DTOs
{
    /// <summary>
    /// Data Transfer Object for Role entities.
    /// Used for transferring role data between the API and client applications.
    /// </summary>
    public class RoleDto : BaseDto
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
    }
}

