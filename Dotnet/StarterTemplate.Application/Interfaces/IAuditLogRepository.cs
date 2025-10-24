using StarterTemplate.Domain.Entities;

namespace StarterTemplate.Application.Interfaces
{
    public interface IAuditLogRepository : IGenericRepository<AuditLog>
    {
        Task<IEnumerable<AuditLog>> GetByEntityAsync(string entityName, string entityId);
        Task<IEnumerable<AuditLog>> GetByUserAsync(int userId, int pageNumber = 1, int pageSize = 10);
        Task<IEnumerable<AuditLog>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, int pageNumber = 1, int pageSize = 10);
        Task<IEnumerable<AuditLog>> GetByActionAsync(string action, int pageNumber = 1, int pageSize = 10);
    }
}
