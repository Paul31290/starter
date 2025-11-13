using System.ComponentModel.DataAnnotations;

namespace StarterTemplate.Application.DTOs
{
    public abstract class BaseDto
    {
        public int Id { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public int? CreatedById { get; set; }
        public DateTimeOffset? ModifiedAt { get; set; }
        public int? ModifiedById { get; set; }
    }
}
