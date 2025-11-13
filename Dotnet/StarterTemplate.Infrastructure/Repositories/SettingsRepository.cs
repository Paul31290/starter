using Microsoft.EntityFrameworkCore;
using StarterTemplate.Application.Interfaces;
using StarterTemplate.Domain.Entities;
using StarterTemplate.Infrastructure.Data;

namespace StarterTemplate.Infrastructure.Repositories
{
    public class SettingsRepository : GenericRepository<Settings>, ISettingsRepository
    {
        public SettingsRepository(StarterTemplateContext context) : base(context)
        {
        }

        public async Task<Settings?> GetByKeyAsync(string key, int? userId = null)
        {
            return await _context.Settings
                .FirstOrDefaultAsync(s => s.Key == key && s.UserId == userId);
        }

        public async Task<IEnumerable<Settings>> GetByCategoryAsync(string category, int? userId = null)
        {
            return await _context.Settings
                .Where(s => s.Category == category && s.UserId == userId)
                .OrderBy(s => s.Key)
                .ToListAsync();
        }

        public async Task<IEnumerable<Settings>> GetUserSettingsAsync(int userId)
        {
            return await _context.Settings
                .Where(s => s.UserId == userId)
                .OrderBy(s => s.Category)
                .ThenBy(s => s.Key)
                .ToListAsync();
        }

        public async Task<IEnumerable<Settings>> GetGlobalSettingsAsync()
        {
            return await _context.Settings
                .Where(s => s.UserId == null)
                .OrderBy(s => s.Category)
                .ThenBy(s => s.Key)
                .ToListAsync();
        }

        public async Task<bool> KeyExistsAsync(string key, int? userId = null)
        {
            return await _context.Settings
                .AnyAsync(s => s.Key == key && s.UserId == userId);
        }

        public async Task<Settings> SetValueAsync(string key, string value, string? description = null, string category = "General", int? userId = null)
        {
            var existingSetting = await GetByKeyAsync(key, userId);
            
            if (existingSetting != null)
            {
                existingSetting.Value = value;
                if (!string.IsNullOrEmpty(description))
                    existingSetting.Description = description;
                existingSetting.Category = category;
                existingSetting.ModifiedAt = DateTime.UtcNow;
                
                _context.Settings.Update(existingSetting);
                return existingSetting;
            }
            else
            {
                var newSetting = new Settings
                {
                    Key = key,
                    Value = value,
                    Description = description,
                    Category = category,
                    UserId = userId,
                    CreatedAt = DateTime.UtcNow
                };
                
                _context.Settings.Add(newSetting);
                return newSetting;
            }
        }
    }
}
