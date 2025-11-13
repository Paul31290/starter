namespace StarterTemplate.Application.DTOs
{
    public class NotificationDto : BaseDto
    {
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string? Severity { get; set; }
        public bool IsRead { get; set; }
        public int? UserId { get; set; }
        public string? UserName { get; set; }
    }
}
