using StarterTemplate.Domain.Entities;

namespace StarterTemplate.Application.Interfaces
{
    public interface ISettingsRepository : IGenericRepository<Settings>
    {
        Task<Settings?> GetByKeyAsync(string key, int? userId = null);
        Task<IEnumerable<Settings>> GetByCategoryAsync(string category, int? userId = null);
        Task<IEnumerable<Settings>> GetUserSettingsAsync(int userId);
        Task<IEnumerable<Settings>> GetGlobalSettingsAsync();
        Task<bool> KeyExistsAsync(string key, int? userId = null);
        Task<Settings> SetValueAsync(string key, string value, string? description = null, string category = "General", int? userId = null);
    }
}
