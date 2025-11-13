using System;
using StarterTemplate.Application.DTOs;
using StarterTemplate.Domain.Entities;

namespace StarterTemplate.Application.Mappings
{
    public class RolePermissionMapper : IEntityMapper<RolePermission, RolePermissionDto>
    {
        private readonly PermissionMapper _permissionMapper;

        public RolePermissionMapper()
        {
            _permissionMapper = new PermissionMapper();
        }

        public RolePermissionDto ToDto(RolePermission entity)
        {
            return new RolePermissionDto
            {
                Id = entity.Id,
                RoleId = entity.RoleId,
                RoleName = entity.Role?.Name,
                PermissionId = entity.PermissionId,
                Permission = entity.Permission != null ? _permissionMapper.ToDto(entity.Permission) : null,
                CreatedAt = entity.CreatedAt == default ? DateTimeOffset.UtcNow : entity.CreatedAt
            };
        }

        public RolePermission ToEntity(RolePermissionDto dto)
        {
            return new RolePermission
            {
                Id = dto.Id,
                RoleId = dto.RoleId,
                PermissionId = dto.PermissionId,
                CreatedAt = dto.CreatedAt
            };
        }

        public void UpdateEntity(RolePermission entity, RolePermissionDto dto)
        {
            entity.RoleId = dto.RoleId;
            entity.PermissionId = dto.PermissionId;
        }
    }
}

