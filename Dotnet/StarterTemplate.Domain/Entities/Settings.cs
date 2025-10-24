
namespace StarterTemplate.Domain.Entities
{
    public class Settings : BaseEntity
    {
        public string Key { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Category { get; set; } = "General";
        public bool IsEncrypted { get; set; } = false;
        public bool IsReadOnly { get; set; } = false;
        
        // Navigation properties
        public int? UserId { get; set; } // If setting is user-specific
        public User? User { get; set; }
    }
}
