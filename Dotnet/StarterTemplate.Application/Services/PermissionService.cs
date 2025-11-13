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
    /// Service for managing permission entities.
    /// Extends the generic CRUD service to provide permission-specific functionality.
    /// </summary>
    public class PermissionService : GenericCrudService<Permission, PermissionDto, PermissionMapper>
    {
        private readonly IPermissionRepository _permissionRepository;

        /// <summary>
        /// Initializes a new instance of the PermissionService class.
        /// </summary>
        /// <param name="repository">The permission repository for data access.</param>
        /// <param name="mapper">The permission mapper for entity-DTO conversion.</param>
        /// <param name="httpContextAccessor">The HTTP context accessor for accessing user information.</param>
        public PermissionService(
            IPermissionRepository repository,
            PermissionMapper mapper,
            IHttpContextAccessor httpContextAccessor)
            : base(repository, mapper, httpContextAccessor)
        {
            _permissionRepository = repository;
        }

        /// <summary>
        /// Gets all permissions for a specific role.
        /// </summary>
        /// <param name="roleId">The ID of the role.</param>
        /// <returns>A collection of permission DTOs.</returns>
        public async Task<IEnumerable<PermissionDto>> GetPermissionsByRoleIdAsync(int roleId)
        {
            var permissions = await _permissionRepository.GetPermissionsByRoleIdAsync(roleId);
            return permissions.Select(_mapper.ToDto);
        }

        /// <summary>
        /// Gets all permissions for a specific user (through their roles).
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <returns>A collection of permission DTOs.</returns>
        public async Task<IEnumerable<PermissionDto>> GetPermissionsByUserIdAsync(int userId)
        {
            var permissions = await _permissionRepository.GetPermissionsByUserIdAsync(userId);
            return permissions.Select(_mapper.ToDto);
        }

        /// <summary>
        /// Checks if a user has a specific permission.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="permissionName">The name of the permission to check.</param>
        /// <returns>True if the user has the permission, otherwise false.</returns>
        public async Task<bool> UserHasPermissionAsync(int userId, string permissionName)
        {
            return await _permissionRepository.UserHasPermissionAsync(userId, permissionName);
        }

        /// <summary>
        /// Provides a function to include navigation properties specific to permission queries.
        /// </summary>
        /// <returns>A function that modifies a query to include permission-specific navigation properties.</returns>
        protected override Func<IQueryable<Permission>, IQueryable<Permission>> IncludeNavigationProperties()
            => query => query
                .Include(p => p.CreatedBy)
                .Include(p => p.ModifiedBy);
    }
}

