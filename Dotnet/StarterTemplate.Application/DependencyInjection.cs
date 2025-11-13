using StarterTemplate.Application.Interfaces;
using StarterTemplate.Application.Services;
using StarterTemplate.Application.Mappings;
using StarterTemplate.Domain.Entities;
using StarterTemplate.Application.DTOs;
using StarterTemplate.Application.Services.GenericCrudService;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Caching.Memory;

namespace StarterTemplate.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            // Register HTTP context accessor
            services.AddHttpContextAccessor();

            // Register memory cache
            services.AddMemoryCache();

            // Register new generic services
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IFileStorageService, FileStorageService>();
            services.AddScoped<ICacheService, CacheService>();
            services.AddScoped<IAuditService, AuditService>();
            services.AddScoped<IValidationService, ValidationService>();

            // Register core mappers
            services.AddScoped<NotificationMapper>();
            services.AddScoped<UserMapper>();
            services.AddScoped<PermissionMapper>();
            services.AddScoped<RolePermissionMapper>();
            services.AddScoped<SettingsMapper>();
            services.AddScoped<AuditLogMapper>();

            // Register core CRUD services
            services.AddScoped<IGenericCrudService<Notification, NotificationDto>, NotificationService>();
            services.AddScoped<IGenericCrudService<User, UserDto>, UserService>();
            services.AddScoped<IGenericCrudService<Permission, PermissionDto>, PermissionService>();
            services.AddScoped<IGenericCrudService<RolePermission, RolePermissionDto>, RolePermissionService>();
            services.AddScoped<IGenericCrudService<Settings, SettingsDto>, SettingsService>();

            // Register specific service interfaces for controllers that use them
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ISettingsService, SettingsService>();
            services.AddScoped<PermissionService>();
            services.AddScoped<RolePermissionService>();

            // Register JWT authentication service
            services.AddScoped<IJwtAuthService, JwtAuthService>();

            return services;
        }
    }
}