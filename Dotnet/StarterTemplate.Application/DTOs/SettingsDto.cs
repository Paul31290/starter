using System.ComponentModel.DataAnnotations;

namespace StarterTemplate.Application.DTOs
{
    public class SettingsDto : BaseDto
    {
        [Required]
        [StringLength(100)]
        public string Key { get; set; } = string.Empty;

        [Required]
        public string Value { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        [StringLength(50)]
        public string Category { get; set; } = "General";

        public bool IsEncrypted { get; set; } = false;
        public bool IsReadOnly { get; set; } = false;
        public int? UserId { get; set; }
    }
}
