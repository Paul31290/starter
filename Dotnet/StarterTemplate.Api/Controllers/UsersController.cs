using StarterTemplate.Api.Attributes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using StarterTemplate.Application.DTOs;
using StarterTemplate.Application.Interfaces;
using StarterTemplate.Application.Services.GenericCrudService;
using StarterTemplate.Domain.Entities;

namespace StarterTemplate.Api.Controllers
{
    /// <summary>
    /// Controller for managing user entities.
    /// Provides REST API endpoints for user management operations.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [RequirePermission("Users_List")]
    [Authorize]
    public class UsersController : BaseController<User, UserDto>
    {
        private readonly IUserService _userService;

        /// <summary>
        /// Initializes a new instance of the UsersController class.
        /// </summary>
        /// <param name="userService">The user service for business logic operations.</param>
        public UsersController(IUserService userService)
            : base(userService)
        {
            _userService = userService;
        }



        /// <summary>
        /// Gets a user by their email address.
        /// </summary>
        /// <param name="email">The email address.</param>
        /// <returns>The user if found; otherwise, NotFound.</returns>
        [HttpGet("email/{email}")]
        public async Task<ActionResult<UserDto>> GetUserByEmail(string email)
        {
            var user = await _userService.GetByEmailAsync(email);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }

        /// <summary>
        /// Gets a user by their username.
        /// </summary>
        /// <param name="userName">The username.</param>
        /// <returns>The user if found; otherwise, NotFound.</returns>
        [HttpGet("username/{userName}")]
        public async Task<ActionResult<UserDto>> GetUserByUserName(string userName)
        {
            var user = await _userService.GetByUserNameAsync(userName);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }

        /// <summary>
        /// Creates a new user.
        /// </summary>
        /// <param name="userDto">The user data.</param>
        /// <returns>The created user.</returns>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public override async Task<ActionResult<UserDto>> Create(UserDto userDto)
        {
            // Check if email is unique
            if (!await _userService.IsEmailUniqueAsync(userDto.Email))
            {
                return BadRequest("Email address is already in use.");
            }

            // Check if username is unique
            if (!await _userService.IsUserNameUniqueAsync(userDto.UserName))
            {
                return BadRequest("Username is already in use.");
            }

            var createdUser = await _userService.CreateAsync(userDto);
            return CreatedAtAction(nameof(Create), new { id = createdUser.Id }, createdUser);
        }

        /// <summary>
        /// Updates an existing user.
        /// </summary>
        /// <param name="id">The user ID.</param>
        /// <param name="userDto">The updated user data.</param>
        /// <returns>The updated user.</returns>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Manager")]
        public override async Task<ActionResult<UserDto>> Update(int id, UserDto userDto)
        {
            if (userDto.Id == null || id != userDto.Id)
            {
                return BadRequest();
            }

            // Check if email is unique (excluding current user)
            var existingUser = await _userService.GetByIdAsync(id);
            if (existingUser == null)
            {
                return NotFound();
            }

            if (existingUser.Email != userDto.Email && !await _userService.IsEmailUniqueAsync(userDto.Email))
            {
                return BadRequest("Email address is already in use.");
            }

            // Check if username is unique (excluding current user)
            if (existingUser.UserName != userDto.UserName && !await _userService.IsUserNameUniqueAsync(userDto.UserName))
            {
                return BadRequest("Username is already in use.");
            }

            var updatedUser = await _userService.UpdateAsync(id, userDto);
            return Ok(updatedUser);
        }

        /// <summary>
        /// Deletes a user.
        /// </summary>
        /// <param name="id">The user ID.</param>
        /// <returns>No content on successful deletion.</returns>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public override async Task<ActionResult<bool>> Delete(int id)
        {
            var deleted = await _userService.DeleteAsync(id);
            if (!deleted)
            {
                return NotFound();
            }
            return NoContent();
        }

        /// <summary>
        /// Checks if an email address is unique.
        /// </summary>
        /// <param name="email">The email address to check.</param>
        /// <returns>True if the email is unique; otherwise, false.</returns>
        [HttpGet("check-email/{email}")]
        public async Task<ActionResult<bool>> CheckEmailUnique(string email)
        {
            var isUnique = await _userService.IsEmailUniqueAsync(email);
            return Ok(isUnique);
        }

        /// <summary>
        /// Checks if a username is unique.
        /// </summary>
        /// <param name="userName">The username to check.</param>
        /// <returns>True if the username is unique; otherwise, false.</returns>
        [HttpGet("check-username/{userName}")]
        public async Task<ActionResult<bool>> CheckUserNameUnique(string userName)
        {
            var isUnique = await _userService.IsUserNameUniqueAsync(userName);
            return Ok(isUnique);
        }

        /// <summary>
        /// Gets all available roles in the system.
        /// </summary>
        /// <returns>A list of all roles.</returns>
        [HttpGet("roles")]
        public async Task<ActionResult<IEnumerable<RoleDto>>> GetAllRoles()
        {
            var roles = await _userService.GetAllRolesAsync();
            return Ok(roles);
        }

        /// <summary>
        /// Gets all roles assigned to a specific user.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <returns>A list of roles assigned to the user.</returns>
        [HttpGet("{userId}/roles")]
        public async Task<ActionResult<IEnumerable<RoleDto>>> GetUserRoles(int userId)
        {
            var roles = await _userService.GetUserRolesAsync(userId);
            return Ok(roles);
        }

        /// <summary>
        /// Assigns roles to a user.
        /// </summary>
        /// <param name="assignRolesDto">The role assignment data.</param>
        /// <returns>Ok if successful; otherwise, BadRequest.</returns>
        [HttpPost("assign-roles")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> AssignRolesToUser([FromBody] AssignRolesDto assignRolesDto)
        {
            var success = await _userService.AssignRolesToUserAsync(assignRolesDto.UserId, assignRolesDto.RoleIds);
            if (!success)
            {
                return BadRequest("Failed to assign roles to user.");
            }
            return Ok();
        }

        /// <summary>
        /// Removes a role from a user.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <param name="roleId">The role ID to remove.</param>
        /// <returns>Ok if successful; otherwise, BadRequest.</returns>
        [HttpDelete("{userId}/roles/{roleId}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> RemoveRoleFromUser(int userId, int roleId)
        {
            var success = await _userService.RemoveRoleFromUserAsync(userId, roleId);
            if (!success)
            {
                return BadRequest("Failed to remove role from user.");
            }
            return Ok();
        }
    }
}
