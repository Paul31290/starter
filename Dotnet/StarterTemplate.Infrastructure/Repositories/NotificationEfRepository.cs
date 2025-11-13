using StarterTemplate.Domain.Entities;
using StarterTemplate.Infrastructure.Data;
using StarterTemplate.Application.Interfaces;

namespace StarterTemplate.Infrastructure.Repositories
{
    public class NotificationEfRepository : GenericRepository<Notification>, INotificationRepository
    {
        public NotificationEfRepository(StarterTemplateContext context) : base(context) { }
    }
}
