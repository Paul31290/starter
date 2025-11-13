namespace StarterTemplate.Application.DTOs
{
    public class AuditLogDto : BaseDto
    {
        public string EntityName { get; set; } = string.Empty;
        public string EntityId { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty;
        public string? OldValues { get; set; }
        public string? NewValues { get; set; }
        public string? Changes { get; set; }
        public string? IpAddress { get; set; }
        public string? UserAgent { get; set; }
        public DateTime Timestamp { get; set; }
        public int? UserId { get; set; }
        public string? UserName { get; set; }
    }
}
