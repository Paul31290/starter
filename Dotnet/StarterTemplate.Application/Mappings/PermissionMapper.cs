using System;
using StarterTemplate.Application.DTOs;
using StarterTemplate.Domain.Entities;

namespace StarterTemplate.Application.Mappings
{
    public class PermissionMapper : IEntityMapper<Permission, PermissionDto>
    {
        public PermissionDto ToDto(Permission entity)
        {
            return new PermissionDto
            {
                Id = entity.Id,
                Name = entity.Name,
                Description = entity.Description,
                Resource = entity.Resource,
                Action = entity.Action,
                CreatedAt = entity.CreatedAt == default ? DateTimeOffset.UtcNow : entity.CreatedAt
            };
        }

        public Permission ToEntity(PermissionDto dto)
        {
            return new Permission
            {
                Id = dto.Id,
                Name = dto.Name,
                Description = dto.Description,
                Resource = dto.Resource,
                Action = dto.Action,
                CreatedAt = dto.CreatedAt
            };
        }

        public void UpdateEntity(Permission entity, PermissionDto dto)
        {
            entity.Name = dto.Name;
            entity.Description = dto.Description;
            entity.Resource = dto.Resource;
            entity.Action = dto.Action;
        }
    }
}

