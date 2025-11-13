using StarterTemplate.Domain.Entities;
using StarterTemplate.Application.DTOs;
using StarterTemplate.Application.Interfaces;
using StarterTemplate.Application.Mappings;
using StarterTemplate.Application.Services.GenericCrudService;
using StarterTemplate.Application.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace StarterTemplate.Application.Services
{
    /// <summary>
    /// Service for managing role-permission relationships.
    /// Extends the generic CRUD service to provide role-permission-specific functionality.
    /// </summary>
    public class RolePermissionService : GenericCrudService<RolePermission, RolePermissionDto, RolePermissionMapper>
    {
        private readonly IRolePermissionRepository _rolePermissionRepository;

        /// <summary>
        /// Initializes a new instance of the RolePermissionService class.
        /// </summary>
        /// <param name="repository">The role-permission repository for data access.</param>
        /// <param name="mapper">The role-permission mapper for entity-DTO conversion.</param>
        /// <param name="httpContextAccessor">The HTTP context accessor for accessing user information.</param>
        public RolePermissionService(
            IRolePermissionRepository repository,
            RolePermissionMapper mapper,
            IHttpContextAccessor httpContextAccessor)
            : base(repository, mapper, httpContextAccessor)
        {
            _rolePermissionRepository = repository;
        }

        /// <summary>
        /// Assigns a permission to a role.
        /// </summary>
        /// <param name="roleId">The ID of the role.</param>
        /// <param name="permissionId">The ID of the permission.</param>
        /// <returns>The created role-permission DTO.</returns>
        public async Task<RolePermissionDto> AssignPermissionToRoleAsync(int roleId, int permissionId)
        {
            var rolePermission = await _rolePermissionRepository.AssignPermissionToRoleAsync(roleId, permissionId);
            return _mapper.ToDto(rolePermission);
        }

        /// <summary>
        /// Removes a permission from a role.
        /// </summary>
        /// <param name="roleId">The ID of the role.</param>
        /// <param name="permissionId">The ID of the permission.</param>
        /// <returns>True if successful, otherwise false.</returns>
        public async Task<bool> RemovePermissionFromRoleAsync(int roleId, int permissionId)
        {
            return await _rolePermissionRepository.RemovePermissionFromRoleAsync(roleId, permissionId);
        }

        /// <summary>
        /// Gets all role permissions for a specific role.
        /// </summary>
        /// <param name="roleId">The ID of the role.</param>
        /// <returns>A collection of role-permission DTOs.</returns>
        public async Task<IEnumerable<RolePermissionDto>> GetRolePermissionsByRoleIdAsync(int roleId)
        {
            var rolePermissions = await _rolePermissionRepository.GetRolePermissionsByRoleIdAsync(roleId);
            return rolePermissions.Select(_mapper.ToDto);
        }

        /// <summary>
        /// Provides a function to include navigation properties specific to role-permission queries.
        /// </summary>
        /// <returns>A function that modifies a query to include role-permission-specific navigation properties.</returns>
        protected override Func<IQueryable<RolePermission>, IQueryable<RolePermission>> IncludeNavigationProperties()
            => query => query
                .Include(rp => rp.Role)
                .Include(rp => rp.Permission)
                .Include(rp => rp.CreatedBy)
                .Include(rp => rp.ModifiedBy);
    }
}

