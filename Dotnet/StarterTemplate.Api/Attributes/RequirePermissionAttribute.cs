using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;
using StarterTemplate.Application.Repositories;

namespace StarterTemplate.Api.Attributes
{
    /// <summary>
    /// Authorization attribute that checks if the user has a specific permission.
    /// This attribute can be applied to controllers or action methods to enforce permission-based access control.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class RequirePermissionAttribute : Attribute, IAsyncAuthorizationFilter
    {
        private readonly string _permissionName;

        /// <summary>
        /// Initializes a new instance of the RequirePermissionAttribute class.
        /// </summary>
        /// <param name="permissionName">The name of the permission required to access the resource.</param>
        public RequirePermissionAttribute(string permissionName)
        {
            _permissionName = permissionName;
        }

        /// <summary>
        /// Called early in the filter pipeline to confirm request is authorized.
        /// </summary>
        /// <param name="context">The authorization filter context.</param>
        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            // Check if user is authenticated
            var user = context.HttpContext.User;
            if (!user.Identity?.IsAuthenticated ?? true)
            {
                context.Result = new UnauthorizedObjectResult(new { message = "User is not authenticated." });
                return;
            }

            // Get user ID from claims
            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            {
                context.Result = new UnauthorizedObjectResult(new { message = "Invalid user ID in token." });
                return;
            }

            // Get permission repository from DI
            var permissionRepository = context.HttpContext.RequestServices.GetService<IPermissionRepository>();
            if (permissionRepository == null)
            {
                context.Result = new StatusCodeResult(500);
                return;
            }

            // Check if user has the required permission
            var hasPermission = await permissionRepository.UserHasPermissionAsync(userId, _permissionName);
            if (!hasPermission)
            {
                context.Result = new ForbidResult();
                return;
            }
        }
    }
}

