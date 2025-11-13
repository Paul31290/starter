using Microsoft.EntityFrameworkCore;
using StarterTemplate.Domain.Entities;

namespace StarterTemplate.Infrastructure.Data
{
    public static class RoleSeeder
    {
        public static void SeedRoles(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Role>().HasData(
                new Role
                {
                    Id = 1,
                    Name = "Admin",
                    Description = "Administrator with full system access",
                    CreatedAt = DateTimeOffset.Parse("2025-10-14 17:15:51.3046563"),

                },
                new Role
                {
                    Id = 2,
                    Name = "Manager",
                    Description = "Manager with limited administrative access",
                    CreatedAt = DateTimeOffset.Parse("2025-10-14 17:15:51.3046563"),

                },
                new Role
                {
                    Id = 3,
                    Name = "User",
                    Description = "Standard user with basic access",
                    CreatedAt = DateTimeOffset.Parse("2025-10-14 17:15:51.3046563"),

                },
                new Role
                {
                    Id = 4,
                    Name = "Viewer",
                    Description = "Read-only access to the system",
                    CreatedAt = DateTimeOffset.Parse("2025-10-14 17:15:51.3046563"),

                }
            );
        }
        public static void SeedUserRoles(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserRole>().HasData(
                new UserRole
                {
                    Id = 1,
                    UserId = 1,
                    RoleId = 1,
                    AssignedAt = DateTime.Parse("2025-10-14 17:15:51.3046563"),
                    CreatedAt = DateTime.Parse("2025-10-14 17:15:51.3046563"),
                    CreatedById = 1,
                    ModifiedAt = null,


                },
                new UserRole
                {
                    Id = 2,
                    UserId = 1,
                    RoleId = 2,
                    AssignedAt = DateTime.Parse("2025-10-14 17:15:51.3046563"),
                    CreatedAt = DateTime.Parse("2025-10-14 17:15:51.3046563"),
                    CreatedById = 1,
                    ModifiedAt = null,


                },
                new UserRole
                {
                    Id = 3,
                    UserId = 1,
                    RoleId = 3,
                    AssignedAt = DateTime.Parse("2025-10-14 17:15:51.3046563"),
                    CreatedAt = DateTime.Parse("2025-10-14 17:15:51.3046563"),
                    CreatedById = 1,
                    ModifiedAt = null,


                },
                new UserRole
                {
                    Id = 4,
                    UserId = 1,
                    RoleId = 4,
                    AssignedAt = DateTime.Parse("2025-10-14 17:15:51.3046563"),
                    CreatedAt = DateTime.Parse("2025-10-14 17:15:51.3046563"),
                    CreatedById = 1,
                    ModifiedAt = null,


                }
            );
        }
    }
}

