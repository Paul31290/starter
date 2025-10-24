using StarterTemplate.Api.Attributes;
using StarterTemplate.Application.DTOs;
using StarterTemplate.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace StarterTemplate.Api.Controllers
{
    /// <summary>
    /// Controller for managing notifications.
    /// </summary>
    [RequirePermission("Notifications_List")]
    public class NotificationsController : BaseController<StarterTemplate.Domain.Entities.Notification, NotificationDto>
    {
        public NotificationsController(IGenericCrudService<StarterTemplate.Domain.Entities.Notification, NotificationDto> service)
            : base(service)
        {
        }

        [HttpPost("{id}/read")]
        public async Task<ActionResult<NotificationDto>> MarkAsRead(int id)
        {
            var dto = await _service.GetByIdAsync(id);
            if (dto == null) return NotFound(new { message = "Notification not found." });
            dto.IsRead = true;
            var updated = await _service.UpdateAsync(id, dto);
            return Ok(updated);
        }

        [HttpGet("unread-count")]
        public async Task<ActionResult<int>> GetUnreadCount([FromQuery] int? userId = null)
        {
            var all = await _service.GetAllAsync();
            var count = all.Count(n => !n.IsRead && (userId == null || (n.UserId != null && n.UserId == userId)));
            return Ok(count);
        }
    }
}
