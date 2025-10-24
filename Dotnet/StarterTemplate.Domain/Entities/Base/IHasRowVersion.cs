namespace StarterTemplate.Domain.Entities
{
    public interface IHasRowVersion
    {
        byte[] RowVersion { get; set; }
    }
}