using StarterTemplate.Domain.Entities;
using StarterTemplate.Application.DTOs;
using StarterTemplate.Application.Interfaces;
using StarterTemplate.Application.Mappings;
using StarterTemplate.Application.Services.GenericCrudService;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace StarterTemplate.Application.Services
{
    /// <summary>
    /// Service for managing user entities with specialized functionality for user-specific operations.
    /// Extends the generic CRUD service to provide user-specific functionality.
    /// </summary>
    public class UserService : GenericCrudService<User, UserDto, UserMapper>, IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IUserRoleRepository _userRoleRepository;

        /// <summary>
        /// Initializes a new instance of the UserService class.
        /// </summary>
        /// <param name="repository">The generic repository for user data access.</param>
        /// <param name="mapper">The user mapper for entity-DTO conversion.</param>
        /// <param name="userRepository">The user repository for user-specific operations.</param>
        /// <param name="roleRepository">The role repository for role operations.</param>
        /// <param name="userRoleRepository">The user-role repository for role assignment operations.</param>
        /// <param name="httpContextAccessor">The HTTP context accessor for accessing user information.</param>
        public UserService(
            IGenericRepository<User> repository, 
            UserMapper mapper, 
            IUserRepository userRepository,
            IRoleRepository roleRepository,
            IUserRoleRepository userRoleRepository,
            IHttpContextAccessor httpContextAccessor)
            : base(repository, mapper, httpContextAccessor)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _userRoleRepository = userRoleRepository;
        }

        /// <summary>
        /// Gets a user by their email address asynchronously.
        /// </summary>
        /// <param name="email">The email address to search for.</param>
        /// <returns>The user DTO if found; otherwise, null.</returns>
        public async Task<UserDto?> GetByEmailAsync(string email)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            return user != null ? _mapper.ToDto(user) : null;
        }

        /// <summary>
        /// Gets a user by their username asynchronously.
        /// </summary>
        /// <param name="userName">The username to search for.</param>
        /// <returns>The user DTO if found; otherwise, null.</returns>
        public async Task<UserDto?> GetByUserNameAsync(string userName)
        {
            var user = await _userRepository.GetByUserNameAsync(userName);
            return user != null ? _mapper.ToDto(user) : null;
        }

        /// <summary>
        /// Checks if an email address is unique in the system asynchronously.
        /// </summary>
        /// <param name="email">The email address to check.</param>
        /// <returns>True if the email is unique; otherwise, false.</returns>
        public async Task<bool> IsEmailUniqueAsync(string email)
        {
            return await _userRepository.IsEmailUniqueAsync(email);
        }

        /// <summary>
        /// Checks if a username is unique in the system asynchronously.
        /// </summary>
        /// <param name="userName">The username to check.</param>
        /// <returns>True if the username is unique; otherwise, false.</returns>
        public async Task<bool> IsUserNameUniqueAsync(string userName)
        {
            return await _userRepository.IsUserNameUniqueAsync(userName);
        }

        /// <summary>
        /// Gets all available roles in the system asynchronously.
        /// </summary>
        /// <returns>A collection of all roles.</returns>
        public async Task<IEnumerable<RoleDto>> GetAllRolesAsync()
        {
            var roles = await _roleRepository.GetAllAsync();
            return roles.Select(r => new RoleDto
            {
                Id = r.Id,
                Name = r.Name,
                Description = r.Description
            });
        }

        /// <summary>
        /// Gets all roles assigned to a specific user asynchronously.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <returns>A collection of roles assigned to the user.</returns>
        public async Task<IEnumerable<RoleDto>> GetUserRolesAsync(int userId)
        {
            var userRoles = await _userRoleRepository.GetByUserIdAsync(userId);
            return userRoles.Where(ur => ur.Role != null).Select(ur => new RoleDto
            {
                Id = ur.Role!.Id,
                Name = ur.Role.Name,
                Description = ur.Role.Description
            });
        }

        /// <summary>
        /// Assigns roles to a user asynchronously.
        /// This method replaces all existing roles with the new ones.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <param name="roleIds">The list of role IDs to assign.</param>
        /// <returns>True if successful; otherwise, false.</returns>
        public async Task<bool> AssignRolesToUserAsync(int userId, List<int> roleIds)
        {
            try
            {
                // Verify user exists
                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null)
                {
                    return false;
                }

                // Get existing user roles
                var existingUserRoles = await _userRoleRepository.GetByUserIdAsync(userId);

                // Remove all existing roles
                foreach (var existingUserRole in existingUserRoles)
                {
                    _userRoleRepository.Remove(existingUserRole);
                }

                // Add new roles
                foreach (var roleId in roleIds)
                {
                    var role = await _roleRepository.GetByIdAsync(roleId);
                    if (role != null)
                    {
                        var userRole = new UserRole
                        {
                            UserId = userId,
                            RoleId = roleId,
                            AssignedAt = DateTime.UtcNow
                        };
                        await _userRoleRepository.AddAsync(userRole);
                    }
                }

                await _userRoleRepository.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Removes a role from a user asynchronously.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <param name="roleId">The role ID to remove.</param>
        /// <returns>True if successful; otherwise, false.</returns>
        public async Task<bool> RemoveRoleFromUserAsync(int userId, int roleId)
        {
            try
            {
                var userRoles = await _userRoleRepository.GetByUserIdAsync(userId);
                var userRole = userRoles.FirstOrDefault(ur => ur.RoleId == roleId);

                if (userRole == null)
                {
                    return false;
                }

                _userRoleRepository.Remove(userRole);
                await _userRoleRepository.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Gets all roles assigned to a specific user asynchronously.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <returns>A collection of roles assigned to the user.</returns>
        public async Task<string?> GetUserProfilePictureAsync(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                {
                    return "No profile picture found for this user";
                }
            return user.ProfilePicture;
        }

        /// <summary>
        /// Update the profile picture of a user asynchronously.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <param name="newProfilePictureDto">The new profile picture as a base64 encoded string from the Data Transfer Object.</param>
        /// <returns>True if successful; otherwise, false.</returns>
        public async Task<bool> UpdateProfilePictureAsync(int userId, NewUserDto newProfilePictureDto)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null)
                {
                    return false;
                }

                user.ProfilePicture = newProfilePictureDto.NewProfilePicture;
                _userRepository.UpdateAsync(user);
                await _userRepository.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Update the username of a user asynchronously.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <param name="newUserNameDto">The new username Data Transfer Object.</param>
        /// <returns>True if successful; otherwise, false.</returns>
        public async Task<bool> UpdateUserNameAsync(int userId, NewUserDto newUserNameDto )
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null)
                {
                    return false;
                }

                user.UserName = newUserNameDto.NewUserName;
                _userRepository.UpdateAsync(user);
                await _userRepository.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }
        /// <summary>
        /// Update the status of a user asynchronously.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <param name="newStatus">The new status.</param>
        /// <returns>True if successful; otherwise, false.</returns>
        public async Task<bool> UpdateUserStatusAsync(int userId, bool newStatus)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null)
                {
                    return false;
                }

                user.IsActive = newStatus;
                _userRepository.UpdateAsync(user);
                await _userRepository.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Provides a function to include navigation properties specific to user queries.
        /// </summary>
        /// <returns>A function that modifies a query to include user-specific navigation properties.</returns>
        protected override Func<IQueryable<User>, IQueryable<User>> IncludeNavigationProperties()
            => query => query.Include(u => u.UserRoles).ThenInclude(ur => ur.Role);
    }
}
