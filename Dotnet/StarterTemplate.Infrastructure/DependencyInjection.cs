using StarterTemplate.Infrastructure.Data;
using StarterTemplate.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StarterTemplate.Application.Interfaces;
using StarterTemplate.Application.Repositories;

namespace StarterTemplate.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // Database
            services.AddDbContext<StarterTemplateContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection")
                )
            );

            // Register IStarterTemplateContext interface to StarterTemplateContext implementation
            services.AddScoped<IStarterTemplateContext, StarterTemplateContext>();

            // Repositories - Core entities only
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IUserRoleRepository, UserRoleRepository>();
            services.AddScoped<IPermissionRepository, PermissionRepository>();
            services.AddScoped<IRolePermissionRepository, RolePermissionRepository>();
            services.AddScoped<INotificationRepository, NotificationEfRepository>();
            
            // New repositories for starter template entities
            services.AddScoped<IAuditLogRepository, AuditLogRepository>();
            services.AddScoped<ISettingsRepository, SettingsRepository>();
            
            // Note: Business-specific repositories removed as part of starter template conversion
            // These will be re-added as needed for specific implementations

            return services;
        }
    }
}