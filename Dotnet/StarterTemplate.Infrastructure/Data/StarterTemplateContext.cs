using StarterTemplate.Domain.Entities;
using StarterTemplate.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace StarterTemplate.Infrastructure.Data
{
    public class StarterTemplateContext : DbContext, IStarterTemplateContext
    {
        public StarterTemplateContext(DbContextOptions<StarterTemplateContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.ConfigureWarnings(warnings =>
                warnings.Ignore(RelationalEventId.PendingModelChangesWarning));
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Settings> Settings { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (entityType.ClrType.GetProperty("CreatedById") != null)
                {
                    modelBuilder.Entity(entityType.ClrType)
                        .HasOne(typeof(User), "CreatedBy")
                        .WithMany()
                        .HasForeignKey("CreatedById")
                        .OnDelete(DeleteBehavior.Restrict);
                }

                if (entityType.ClrType.GetProperty("ModifiedById") != null)
                {
                    modelBuilder.Entity(entityType.ClrType)
                        .HasOne(typeof(User), "ModifiedBy")
                        .WithMany()
                        .HasForeignKey("ModifiedById")
                        .OnDelete(DeleteBehavior.Restrict);
                }
            }


            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.UserName)
                .IsUnique();

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(ur => ur.RoleId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserRole>()
                .HasIndex(ur => new { ur.UserId, ur.RoleId })
                .IsUnique();

            modelBuilder.Entity<RolePermission>()
                .HasOne(rp => rp.Role)
                .WithMany(r => r.RolePermissions)
                .HasForeignKey(rp => rp.RoleId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<RolePermission>()
                .HasOne(rp => rp.Permission)
                .WithMany(p => p.RolePermissions)
                .HasForeignKey(rp => rp.PermissionId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<RolePermission>()
                .HasIndex(rp => new { rp.RoleId, rp.PermissionId })
                .IsUnique();

            modelBuilder.Entity<Permission>()
                .HasIndex(p => p.Name)
                .IsUnique();

            modelBuilder.Entity<Notification>()
                .HasOne(n => n.User)
                .WithMany()
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<AuditLog>()
                .HasOne(a => a.User)
                .WithMany()
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<AuditLog>()
                .HasIndex(a => new { a.EntityName, a.EntityId });

            modelBuilder.Entity<AuditLog>()
                .HasIndex(a => a.Timestamp);

            modelBuilder.Entity<AuditLog>()
                .HasIndex(a => a.UserId);

            modelBuilder.Entity<Settings>()
                .HasOne(s => s.User)
                .WithMany()
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Settings>()
                .HasIndex(s => new { s.Key, s.UserId })
                .IsUnique();

            modelBuilder.Entity<Settings>()
                .HasIndex(s => s.Category);

            modelBuilder.Entity<RefreshToken>()
                .HasOne(rt => rt.User)
                .WithMany()
                .HasForeignKey(rt => rt.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<RefreshToken>()
                .HasIndex(rt => rt.Token)
                .IsUnique();

            modelBuilder.Entity<RefreshToken>()
                .HasIndex(rt => rt.UserId);

            modelBuilder.Entity<RefreshToken>()
                .HasIndex(rt => rt.ExpiresAt);

            SeedCoreData(modelBuilder);
        }

        private void SeedCoreData(ModelBuilder modelBuilder)
        {
            var seedDate = new DateTimeOffset(2025, 1, 1, 0, 0, 0, TimeSpan.Zero);

            var adminUserId = 1;
            modelBuilder.Entity<User>().HasData(new User
            {
                Id = adminUserId,
                UserName = "admin",
                Email = "admin@startertemplate.com",
                PasswordHash = "AQAAAAEAACcQAAAAEDefaultPasswordHashForSeeding",
                FirstName = "Admin",
                LastName = "User",
                IsActive = true,
                CreatedAt = seedDate,
                CreatedById = adminUserId
            });

            var adminRoleId = 1;
            var userRoleId = 2;

            modelBuilder.Entity<Role>().HasData(
                new Role { Id = adminRoleId, Name = "Administrator", Description = "Full system access", CreatedAt = seedDate, CreatedById = adminUserId },
                new Role { Id = userRoleId, Name = "User", Description = "Standard user access", CreatedAt = seedDate, CreatedById = adminUserId }
            );

            var permissions = new[]
            {
                new Permission { Id = 1, Name = "Users.Read", Description = "Read users", CreatedAt = seedDate, CreatedById = adminUserId },
                new Permission { Id = 2, Name = "Users.Create", Description = "Create users", CreatedAt = seedDate, CreatedById = adminUserId },
                new Permission { Id = 3, Name = "Users.Update", Description = "Update users", CreatedAt = seedDate, CreatedById = adminUserId },
                new Permission { Id = 4, Name = "Users.Delete", Description = "Delete users", CreatedAt = seedDate, CreatedById = adminUserId },
                new Permission { Id = 5, Name = "Roles.Read", Description = "Read roles", CreatedAt = seedDate, CreatedById = adminUserId },
                new Permission { Id = 6, Name = "Roles.Create", Description = "Create roles", CreatedAt = seedDate, CreatedById = adminUserId },
                new Permission { Id = 7, Name = "Roles.Update", Description = "Update roles", CreatedAt = seedDate, CreatedById = adminUserId },
                new Permission { Id = 8, Name = "Roles.Delete", Description = "Delete roles", CreatedAt = seedDate, CreatedById = adminUserId },
                new Permission { Id = 9, Name = "Settings.Read", Description = "Read settings", CreatedAt = seedDate, CreatedById = adminUserId },
                new Permission { Id = 10, Name = "Settings.Update", Description = "Update settings", CreatedAt = seedDate, CreatedById = adminUserId },
                new Permission { Id = 11, Name = "AuditLog.Read", Description = "Read audit logs", CreatedAt = seedDate, CreatedById = adminUserId }
            };

            modelBuilder.Entity<Permission>().HasData(permissions);

            modelBuilder.Entity<UserRole>().HasData(new UserRole
            {
                Id = 1,
                UserId = adminUserId,
                RoleId = adminRoleId,
                CreatedAt = seedDate,
                CreatedById = adminUserId
            });

            var rolePermissions = permissions.Select((p, index) => new RolePermission
            {
                Id = index + 1,
                RoleId = adminRoleId,
                PermissionId = p.Id,
                CreatedAt = seedDate,
                CreatedById = adminUserId
            }).ToArray();

            modelBuilder.Entity<RolePermission>().HasData(rolePermissions);

            // Seed default settings
            modelBuilder.Entity<Settings>().HasData(
                new Settings
                {
                    Id = 1,
                    Key = "ApplicationName",
                    Value = "StarterTemplate",
                    Description = "The name of the application",
                    Category = "General",
                    IsReadOnly = false,
                    CreatedAt = seedDate,
                    CreatedById = adminUserId
                },
                new Settings
                {
                    Id = 2,
                    Key = "MaxLoginAttempts",
                    Value = "5",
                    Description = "Maximum number of login attempts before account lockout",
                    Category = "Security",
                    IsReadOnly = false,
                    CreatedAt = seedDate,
                    CreatedById = adminUserId
                },
                new Settings
                {
                    Id = 3,
                    Key = "SessionTimeout",
                    Value = "30",
                    Description = "Session timeout in minutes",
                    Category = "Security",
                    IsReadOnly = false,
                    CreatedAt = seedDate,
                    CreatedById = adminUserId
                }
            );
        }
    }
}