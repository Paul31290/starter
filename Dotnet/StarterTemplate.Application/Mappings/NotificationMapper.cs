using System;
using StarterTemplate.Application.DTOs;
using StarterTemplate.Domain.Entities;

namespace StarterTemplate.Application.Mappings
{
    public class NotificationMapper : IEntityMapper<Notification, NotificationDto>
    {
        public NotificationDto ToDto(Notification entity)
        {
            return new NotificationDto
            {
                Id = entity.Id,
                Title = entity.Title,
                Message = entity.Message,
                Severity = entity.Severity,
                IsRead = entity.IsRead,
                UserId = entity.UserId,
                UserName = entity.User != null ? entity.User.UserName : null,
                CreatedAt = entity.CreatedAt == default ? DateTimeOffset.UtcNow : entity.CreatedAt
            };
        }

        public Notification ToEntity(NotificationDto dto)
        {
            return new Notification
            {
                Id = dto.Id,
                Title = dto.Title,
                Message = dto.Message,
                Severity = dto.Severity,
                IsRead = dto.IsRead,
                UserId = dto.UserId,
                CreatedAt = dto.CreatedAt
            };
        }

        public void UpdateEntity(Notification entity, NotificationDto dto)
        {
            entity.Title = dto.Title;
            entity.Message = dto.Message;
            entity.Severity = dto.Severity;
            entity.IsRead = dto.IsRead;
            entity.UserId = dto.UserId;
        }
    }
}
