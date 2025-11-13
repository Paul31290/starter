using System.ComponentModel.DataAnnotations;

namespace StarterTemplate.Application.DTOs
{
    /// <summary>
    /// Data Transfer Object for assigning roles to a user.
    /// </summary>
    public class AssignRolesDto
    {
        /// <summary>
        /// Gets or sets the ID of the user to assign roles to.
        /// </summary>
        [Required]
        public int UserId { get; set; }

        /// <summary>
        /// Gets or sets the list of role IDs to assign to the user.
        /// </summary>
        [Required]
        [MinLength(1, ErrorMessage = "At least one role must be specified")]
        public List<int> RoleIds { get; set; } = new List<int>();
    }
}
