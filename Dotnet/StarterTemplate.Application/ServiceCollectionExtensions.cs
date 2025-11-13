using StarterTemplate.Application.DTOs;
using StarterTemplate.Application.Interfaces;
using StarterTemplate.Application.Services.GenericCrudService;
using StarterTemplate.Domain.Entities;
using Microsoft.Extensions.DependencyInjection;

namespace StarterTemplate.Application
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddGenericCrudService<TEntity, TDto, TMapper>(
            this IServiceCollection services)
            where TEntity : BaseEntity
            where TDto : BaseDto
            where TMapper : class, IEntityMapper<TEntity, TDto>
        {
            services.AddScoped<TMapper>();
            services.AddScoped<IGenericCrudService<TEntity, TDto>, GenericCrudService<TEntity, TDto, TMapper>>();
            return services;
        }
    }
} 