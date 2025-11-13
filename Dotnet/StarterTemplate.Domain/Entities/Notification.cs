using System;

namespace StarterTemplate.Domain.Entities
{
    /// <summary>
    /// Represents a simple notification to be displayed to users.
    /// </summary>
    public class Notification : BaseEntity
    {
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string? Severity { get; set; }
        public bool IsRead { get; set; }
        public int? UserId { get; set; }
        public User? User { get; set; }
    }
}
