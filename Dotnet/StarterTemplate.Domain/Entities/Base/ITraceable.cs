namespace StarterTemplate.Domain.Entities
{
    public interface ITraceable
    {
        DateTimeOffset CreatedAt { get; set; }
        int? CreatedById { get; set; }
        User? CreatedBy { get; set; }
        DateTimeOffset? ModifiedAt { get; set; }
        int? ModifiedById { get; set; }
        User? ModifiedBy { get; set; }
    }

}
