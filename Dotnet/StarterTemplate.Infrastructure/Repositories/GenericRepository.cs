using StarterTemplate.Application.Interfaces;
using StarterTemplate.Application.DTOs;
using StarterTemplate.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace StarterTemplate.Infrastructure.Repositories
{
    /// <summary>
    /// Generic repository implementation that provides common data access operations for entities.
    /// This class implements the IGenericRepository interface using Entity Framework Core.
    /// </summary>
    /// <typeparam name="T">The type of entity this repository handles.</typeparam>
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        /// <summary>
        /// The database context used for data access operations.
        /// </summary>
        protected readonly StarterTemplateContext _context;

        /// <summary>
        /// The Entity Framework DbSet for the entity type T.
        /// </summary>
        protected readonly DbSet<T> _dbSet;

        /// <summary>
        /// Initializes a new instance of the GenericRepository class.
        /// </summary>
        /// <param name="context">The database context to use for data access.</param>
        public GenericRepository(StarterTemplateContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        /// <summary>
        /// Retrieves an entity by its unique identifier asynchronously with specified related entities included.
        /// </summary>
        /// <param name="id">The unique identifier of the entity to retrieve.</param>
        /// <param name="includes">Array of expressions specifying which related entities to include.</param>
        /// <returns>The entity if found; otherwise, null.</returns>
        public async Task<T?> GetByIdAsync(int id, params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _dbSet;

            foreach (var include in includes)
                query = query.Include(include);

            return await query.FirstOrDefaultAsync(e => EF.Property<int>(e, "Id") == id);
        }

        /// <summary>
        /// Retrieves an entity by its unique identifier asynchronously.
        /// </summary>
        /// <param name="id">The unique identifier of the entity to retrieve.</param>
        /// <param name="include">Optional function to include related entities in the query.</param>
        /// <returns>The entity if found; otherwise, null.</returns>
        public async Task<T?> GetByIdAsync(int id, Func<IQueryable<T>, IQueryable<T>>? include = null)
        {
            IQueryable<T> query = _dbSet;

            if (include != null)
                query = include(query);

            return await query.FirstOrDefaultAsync(e => EF.Property<int>(e, "Id") == id);
        }

        /// <summary>
        /// Retrieves all entities asynchronously with optional filtering, ordering, and inclusion of related entities.
        /// </summary>
        /// <param name="filter">Optional filter expression to apply to the query.</param>
        /// <param name="orderBy">Optional ordering function to apply to the query.</param>
        /// <param name="include">Optional function to include related entities in the query.</param>
        /// <returns>A collection of entities matching the specified criteria.</returns>
        public async Task<IEnumerable<T>> GetAllAsync(
            Expression<Func<T, bool>>? filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
            Func<IQueryable<T>, IQueryable<T>>? include = null)
        {
            IQueryable<T> query = _dbSet;

            if (filter != null)
                query = query.Where(filter);

            if (include != null)
                query = include(query);

            if (orderBy != null)
                query = orderBy(query);

            return await query.ToListAsync();
        }

        /// <summary>
        /// Retrieves entities in a paginated format with optional filtering, ordering, and inclusion of related entities.
        /// </summary>
        /// <param name="paginationRequest">The pagination parameters including page number, page size, and sorting options.</param>
        /// <param name="filter">Optional filter expression to apply to the query.</param>
        /// <param name="orderBy">Optional ordering function to apply to the query.</param>
        /// <param name="include">Optional function to include related entities in the query.</param>
        /// <returns>A paginated response containing the entities and pagination metadata.</returns>
        public async Task<PaginatedResponseDto<T>> GetPagedAsync(
            PaginationRequestDto paginationRequest,
            Expression<Func<T, bool>>? filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
            Func<IQueryable<T>, IQueryable<T>>? include = null)
        {
            IQueryable<T> query = _dbSet;

            if (filter != null)
                query = query.Where(filter);

            if (include != null)
                query = include(query);

            // Apply search if provided
            if (!string.IsNullOrWhiteSpace(paginationRequest.SearchTerm))
            {
                // Note: This is a basic implementation. You might want to implement more sophisticated search logic
                // based on your specific entity properties
                query = ApplySearch(query, paginationRequest.SearchTerm);
            }

            // Get total count before applying pagination
            var totalCount = await query.CountAsync();

            // Apply sorting
            if (orderBy != null)
                query = orderBy(query);
            else if (!string.IsNullOrWhiteSpace(paginationRequest.SortBy))
            {
                query = ApplySorting(query, paginationRequest.SortBy, paginationRequest.SortDirection);
            }

            // Apply pagination
            var skip = (paginationRequest.PageNumber - 1) * paginationRequest.PageSize;
            var items = await query.Skip(skip).Take(paginationRequest.PageSize).ToListAsync();

            var totalPages = (int)Math.Ceiling((double)totalCount / paginationRequest.PageSize);

            return new PaginatedResponseDto<T>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = paginationRequest.PageNumber,
                PageSize = paginationRequest.PageSize,
                TotalPages = totalPages,
                HasPrevious = paginationRequest.PageNumber > 1,
                HasNext = paginationRequest.PageNumber < totalPages
            };
        }

        /// <summary>
        /// Finds entities that match the specified expression asynchronously.
        /// </summary>
        /// <param name="expression">The expression to filter entities by.</param>
        /// <returns>A collection of entities matching the expression.</returns>
        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> expression)
        {
            return await _dbSet.Where(expression).ToListAsync();
        }

        /// <summary>
        /// Adds a new entity to the repository asynchronously.
        /// </summary>
        /// <param name="entity">The entity to add.</param>
        /// <returns>The added entity with any generated values (such as ID).</returns>
        public async Task<T> AddAsync(T entity)
        {
            var entry = await _dbSet.AddAsync(entity);
            return entry.Entity;
        }

        /// <summary>
        /// Updates an existing entity in the repository.
        /// </summary>
        /// <param name="entity">The entity to update.</param>
        /// <returns>The updated entity.</returns>
        public T UpdateAsync(T entity)
        {
            _dbSet.Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;
            return entity;
        }

        /// <summary>
        /// Removes an entity from the repository.
        /// </summary>
        /// <param name="entity">The entity to remove.</param>
        public void Remove(T entity)
        {
            if (_context.Entry(entity).State == EntityState.Detached)
            {
                _dbSet.Attach(entity);
            }
            _dbSet.Remove(entity);
        }

        /// <summary>
        /// Saves all changes made to entities in the repository asynchronously.
        /// </summary>
        /// <returns>The number of affected records.</returns>
        public async Task<int> SaveChangesAsync()
        {
            Console.WriteLine($"[GenericRepository] SaveChangesAsync - Saving changes for {typeof(T).Name}");
            var result = await _context.SaveChangesAsync();
            Console.WriteLine($"[GenericRepository] SaveChangesAsync - Saved {result} changes for {typeof(T).Name}");
            return result;
        }

        /// <summary>
        /// Gets a queryable interface for the entity set.
        /// This allows for complex LINQ queries to be built.
        /// </summary>
        /// <returns>A queryable interface for the entity set.</returns>
        public IQueryable<T> GetQueryable()
        {
            return _dbSet;
        }

        /// <summary>
        /// Applies search functionality to the query based on string properties.
        /// This is a basic implementation that searches in string properties.
        /// </summary>
        /// <param name="query">The query to apply search to.</param>
        /// <param name="searchTerm">The search term to look for.</param>
        /// <returns>The modified query with search applied.</returns>
        private IQueryable<T> ApplySearch(IQueryable<T> query, string searchTerm)
        {
            // This is a basic implementation that searches in string properties
            // You might want to override this in specific repositories for more sophisticated search
            var parameter = Expression.Parameter(typeof(T), "x");
            var searchExpression = Expression.Constant(searchTerm.ToLower());

            // Get all string properties
            var stringProperties = typeof(T).GetProperties()
                .Where(p => p.PropertyType == typeof(string) && p.CanRead)
                .ToList();

            if (!stringProperties.Any())
                return query;

            var searchExpressions = new List<Expression>();

            foreach (var property in stringProperties)
            {
                var propertyExpression = Expression.Property(parameter, property);
                var toLowerMethod = typeof(string).GetMethod("ToLower", Type.EmptyTypes);
                var toLowerExpression = Expression.Call(propertyExpression, toLowerMethod);
                var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });
                var containsExpression = Expression.Call(toLowerExpression, containsMethod, searchExpression);
                searchExpressions.Add(containsExpression);
            }

            if (!searchExpressions.Any())
                return query;

            // Combine all search expressions with OR
            var combinedExpression = searchExpressions.Aggregate((current, next) =>
                Expression.Or(current, next));

            var lambda = Expression.Lambda<Func<T, bool>>(combinedExpression, parameter);
            return query.Where(lambda);
        }

        /// <summary>
        /// Applies sorting to the query based on the specified property and direction.
        /// </summary>
        /// <param name="query">The query to apply sorting to.</param>
        /// <param name="sortBy">The property name to sort by.</param>
        /// <param name="sortDirection">The sort direction ("asc" or "desc").</param>
        /// <returns>The modified query with sorting applied.</returns>
        private IQueryable<T> ApplySorting(IQueryable<T> query, string sortBy, string? sortDirection)
        {
            var parameter = Expression.Parameter(typeof(T), "x");
            var property = typeof(T).GetProperty(sortBy, System.Reflection.BindingFlags.IgnoreCase | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);

            if (property == null)
                return query;

            var propertyExpression = Expression.Property(parameter, property);
            var lambda = Expression.Lambda(propertyExpression, parameter);

            var methodName = sortDirection?.ToLower() == "desc" ? "OrderByDescending" : "OrderBy";
            var method = typeof(Queryable).GetMethods()
                .Where(m => m.Name == methodName && m.GetParameters().Length == 2)
                .First()
                .MakeGenericMethod(typeof(T), property.PropertyType);

            return (IQueryable<T>)method.Invoke(null, new object[] { query, lambda });
        }

        /// <summary>
        /// Exports entities to CSV format asynchronously with optional filtering and ordering.
        /// </summary>
        /// <param name="filter">Optional filter expression to apply to the query.</param>
        /// <param name="orderBy">Optional ordering function to apply to the query.</param>
        /// <param name="include">Optional function to include related entities in the query.</param>
        /// <param name="searchTerm">Optional search term to filter entities.</param>
        /// <returns>A CSV string containing the exported data.</returns>
        public async Task<string> ExportToCsvAsync(
            Expression<Func<T, bool>>? filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
            Func<IQueryable<T>, IQueryable<T>>? include = null,
            string? searchTerm = null)
        {
            IQueryable<T> query = _dbSet;

            if (filter != null)
                query = query.Where(filter);

            if (include != null)
                query = include(query);

            if (!string.IsNullOrWhiteSpace(searchTerm))
                query = ApplySearch(query, searchTerm);

            if (orderBy != null)
                query = orderBy(query);

            var entities = await query.ToListAsync();
            return ConvertToCsv(entities);
        }

        /// <summary>
        /// Converts a collection of entities to CSV format.
        /// </summary>
        /// <param name="entities">The entities to convert to CSV.</param>
        /// <returns>A CSV string containing the exported data.</returns>
        private string ConvertToCsv(IEnumerable<T> entities)
        {
            if (!entities.Any())
                return string.Empty;

            var properties = typeof(T).GetProperties()
                .Where(p => p.CanRead && !p.GetCustomAttributes(typeof(System.ComponentModel.DataAnnotations.Schema.NotMappedAttribute), true).Any())
                .ToList();

            if (!properties.Any())
                return string.Empty;

            var csv = new System.Text.StringBuilder();

            // Add header row
            var headers = properties.Select(p => EscapeCsvField(p.Name));
            csv.AppendLine(string.Join(",", headers));

            // Add data rows
            foreach (var entity in entities)
            {
                var values = properties.Select(p => EscapeCsvField(GetPropertyValue(entity, p)));
                csv.AppendLine(string.Join(",", values));
            }

            return csv.ToString();
        }

        /// <summary>
        /// Gets the value of a property from an entity, handling null values and complex types.
        /// </summary>
        /// <param name="entity">The entity to get the property value from.</param>
        /// <param name="property">The property to get the value of.</param>
        /// <returns>The property value as a string.</returns>
        private string GetPropertyValue(T entity, System.Reflection.PropertyInfo property)
        {
            try
            {
                var value = property.GetValue(entity);

                if (value == null)
                    return string.Empty;

                // Handle different types
                if (value is DateTime dateTime)
                    return dateTime.ToString("yyyy-MM-dd HH:mm:ss");

                if (value is bool boolValue)
                    return boolValue.ToString().ToLower();

                if (value is decimal decimalValue)
                    return decimalValue.ToString("F2");

                if (value is double doubleValue)
                    return doubleValue.ToString("F2");

                if (value is float floatValue)
                    return floatValue.ToString("F2");

                if (value is int intValue)
                    return intValue.ToString();

                if (value is long longValue)
                    return longValue.ToString();

                // For complex objects, try to get a meaningful string representation
                if (property.PropertyType.IsClass && property.PropertyType != typeof(string))
                {
                    // Try to get an Id property or Name property
                    var idProperty = property.PropertyType.GetProperty("Id");
                    if (idProperty != null)
                        return idProperty.GetValue(value)?.ToString() ?? string.Empty;

                    var nameProperty = property.PropertyType.GetProperty("Name");
                    if (nameProperty != null)
                        return nameProperty.GetValue(value)?.ToString() ?? string.Empty;

                    return value.ToString();
                }

                return value.ToString();
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Escapes a field value for CSV format.
        /// </summary>
        /// <param name="field">The field value to escape.</param>
        /// <returns>The escaped field value.</returns>
        private string EscapeCsvField(string field)
        {
            if (string.IsNullOrEmpty(field))
                return string.Empty;

            // If the field contains comma, quote, or newline, wrap it in quotes and escape internal quotes
            if (field.Contains(',') || field.Contains('"') || field.Contains('\n') || field.Contains('\r'))
            {
                return $"\"{field.Replace("\"", "\"\"")}\"";
            }

            return field;
        }
    }
}
