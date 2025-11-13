using StarterTemplate.Application.DTOs;
using StarterTemplate.Application.Interfaces;
using StarterTemplate.Application.Mappings;
using StarterTemplate.Application.Services.GenericCrudService;
using StarterTemplate.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace StarterTemplate.Application.Services
{
    /// <summary>
    /// CRUD service for Notifications using the generic CRUD pattern.
    /// </summary>
    public class NotificationService : GenericCrudService<Notification, NotificationDto, NotificationMapper>
    {
        public NotificationService(
            IGenericRepository<Notification> repository, 
            NotificationMapper mapper,
            IHttpContextAccessor httpContextAccessor)
            : base(repository, mapper, httpContextAccessor)
        {
        }

        /// <summary>
        /// Include related User to expose UserName in DTOs.
        /// </summary>
        protected override Func<IQueryable<Notification>, IQueryable<Notification>> IncludeNavigationProperties()
            => query => query.Include(n => n.User);
    }
}
