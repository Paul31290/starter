using StarterTemplate.Application.DTOs;
using StarterTemplate.Domain.Entities;

namespace StarterTemplate.Application.Mappings
{
    /// <summary>
    /// Mapper class for converting between AuditLog entities and AuditLogDto objects.
    /// Handles the mapping of audit log data.
    /// </summary>
    public class AuditLogMapper : IEntityMapper<AuditLog, AuditLogDto>
    {
        /// <summary>
        /// Converts an AuditLog entity to an AuditLogDto object.
        /// </summary>
        /// <param name="entity">The AuditLog entity to convert.</param>
        /// <returns>An AuditLogDto object containing the mapped data.</returns>
        public AuditLogDto ToDto(AuditLog entity)
        {
            return new AuditLogDto
            {
                Id = entity.Id,
                EntityName = entity.EntityName,
                EntityId = entity.EntityId,
                Action = entity.Action,
                Changes = entity.Changes,
                IpAddress = entity.IpAddress,
                UserAgent = entity.UserAgent,
                Timestamp = entity.Timestamp,
                UserId = entity.UserId,
                UserName = entity.User?.UserName,
                CreatedAt = entity.CreatedAt,
                CreatedById = entity.CreatedById,
                ModifiedAt = entity.ModifiedAt,
                ModifiedById = entity.ModifiedById
            };
        }

        /// <summary>
        /// Converts an AuditLogDto object to an AuditLog entity.
        /// </summary>
        /// <param name="dto">The AuditLogDto object to convert.</param>
        /// <returns>An AuditLog entity containing the mapped data.</returns>
        public AuditLog ToEntity(AuditLogDto dto)
        {
            return new AuditLog
            {
                Id = dto.Id,
                EntityName = dto.EntityName,
                EntityId = dto.EntityId,
                Action = dto.Action,
                Changes = dto.Changes,
                IpAddress = dto.IpAddress,
                UserAgent = dto.UserAgent,
                Timestamp = dto.Timestamp,
                UserId = dto.UserId,
                CreatedAt = dto.CreatedAt,
                CreatedById = dto.CreatedById,
                ModifiedAt = dto.ModifiedAt,
                ModifiedById = dto.ModifiedById
            };
        }

        /// <summary>
        /// Updates an existing AuditLog entity with data from an AuditLogDto object.
        /// </summary>
        /// <param name="entity">The AuditLog entity to update.</param>
        /// <param name="dto">The AuditLogDto object containing the updated data.</param>
        public void UpdateEntity(AuditLog entity, AuditLogDto dto)
        {
            entity.EntityName = dto.EntityName;
            entity.EntityId = dto.EntityId;
            entity.Action = dto.Action;
            entity.Changes = dto.Changes;
            entity.IpAddress = dto.IpAddress;
            entity.UserAgent = dto.UserAgent;
            entity.Timestamp = dto.Timestamp;
            entity.UserId = dto.UserId;
            entity.ModifiedAt = dto.ModifiedAt;
            entity.ModifiedById = dto.ModifiedById;
        }
    }
}
