namespace StarterTemplate.Application.DTOs
{
    /// <summary>
    /// Data Transfer Object for Permission entity.
    /// </summary>
    public class PermissionDto : BaseDto
    {
        /// <summary>
        /// Gets or sets the unique name/code of the permission.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the description of the permission.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Gets or sets the resource/module this permission applies to.
        /// </summary>
        public string? Resource { get; set; }

        /// <summary>
        /// Gets or sets the action this permission allows.
        /// </summary>
        public string? Action { get; set; }
    }
}

