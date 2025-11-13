using Microsoft.EntityFrameworkCore;
using StarterTemplate.Domain.Entities;

namespace StarterTemplate.Infrastructure.Data
{
    public static class PermissionSeeder
    {
        public static void SeedPermissions(ModelBuilder modelBuilder)
        {
            var permissions = new List<Permission>();
            int permissionId = 1;

            var resources = new[]
            {
                "Products", "Dishes", "DishProducts", "DishQuantities", "ProductTypes", 
                "Units", "CookingTimes", "Currencies", "TemperatureUnits", 
                "DailyCostAndProfit", "DailyCostAndProfitStatus", 
                "ProfitabilityThresholds", "Users", "Notifications", "Permissions", "RolePermissions"
            };

            var actions = new[] { "List", "View", "Create", "Update", "Delete", "Export" };

            foreach (var resource in resources)
            {
                foreach (var action in actions)
                {
                    permissions.Add(new Permission
                    {
                        Id = permissionId++,
                        Name = $"{resource}_{action}",
                        Description = $"Permission to {action.ToLower()} {resource.ToLower()}",
                        Resource = resource,
                        Action = action,
                        CreatedAt = DateTimeOffset.Parse("2025-10-14 17:15:51.3046563")
                    });
                }
            }

            modelBuilder.Entity<Permission>().HasData(permissions);
        }

        public static void SeedRolePermissions(ModelBuilder modelBuilder)
        {
            var rolePermissions = new List<RolePermission>();
            int rolePermissionId = 1;

            for (int permissionId = 1; permissionId <= 96; permissionId++)
            {
                rolePermissions.Add(new RolePermission
                {
                    Id = rolePermissionId++,
                    RoleId = 1,
                    PermissionId = permissionId,
                    CreatedAt = DateTimeOffset.Parse("2025-10-14 17:15:51.3046563")
                });
            }

            var managerExcludedPermissions = new[] { "Users_Delete", "Permissions_Delete", "RolePermissions_Delete" };
            for (int permissionId = 1; permissionId <= 96; permissionId++)
            {
                var permission = GetPermissionNameById(permissionId);
                if (!managerExcludedPermissions.Contains(permission))
                {
                    rolePermissions.Add(new RolePermission
                    {
                        Id = rolePermissionId++,
                        RoleId = 2,
                        PermissionId = permissionId,
                        CreatedAt = DateTimeOffset.Parse("2025-10-14 17:15:51.3046563")
                    });
                }
            }

            var userResources = new[]
            {
                "Products", "Dishes", "DishProducts", "DishQuantities", "ProductTypes",
                "Units", "CookingTimes", "Currencies", "TemperatureUnits",
                "DailyCostAndProfit", "DailyCostAndProfitStatus",
                "ProfitabilityThresholds", "Notifications"
            };
            var userActions = new[] { "List", "View", "Create", "Update", "Delete", "Export" };
            
            for (int permissionId = 1; permissionId <= 96; permissionId++)
            {
                var permission = GetPermissionNameById(permissionId);
                if (userResources.Any(r => permission.StartsWith(r)) && 
                    userActions.Any(a => permission.EndsWith(a)))
                {
                    rolePermissions.Add(new RolePermission
                    {
                        Id = rolePermissionId++,
                        RoleId = 3,
                        PermissionId = permissionId,
                        CreatedAt = DateTimeOffset.Parse("2025-10-14 17:15:51.3046563")
                    });
                }
            }

            var viewerActions = new[] { "List", "View" };
            for (int permissionId = 1; permissionId <= 96; permissionId++)
            {
                var permission = GetPermissionNameById(permissionId);
                if (viewerActions.Any(a => permission.EndsWith(a)))
                {
                    rolePermissions.Add(new RolePermission
                    {
                        Id = rolePermissionId++,
                        RoleId = 4,
                        PermissionId = permissionId,
                        CreatedAt = DateTimeOffset.Parse("2025-10-14 17:15:51.3046563")
                    });
                }
            }

            modelBuilder.Entity<RolePermission>().HasData(rolePermissions);
        }

        private static string GetPermissionNameById(int permissionId)
        {
            var resources = new[]
            {
                "Products", "Dishes", "DishProducts", "DishQuantities", "ProductTypes",
                "Units", "CookingTimes", "Currencies", "TemperatureUnits",
                "DailyCostAndProfit", "DailyCostAndProfitStatus",
                "ProfitabilityThresholds", "Users", "Notifications", "Permissions", "RolePermissions"
            };

            var actions = new[] { "List", "View", "Create", "Update", "Delete", "Export" };

            int resourceIndex = (permissionId - 1) / 6;
            int actionIndex = (permissionId - 1) % 6;

            return $"{resources[resourceIndex]}_{actions[actionIndex]}";
        }
    }
}

