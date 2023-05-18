using Models.Consts;
using Models.DbEntities;
using Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Services.Repos
{
    /// <summary>
    /// Represents a repository that provides CRUD operations for entities of type T.
    /// </summary>
    /// <typeparam name="T">The type of entity to work with.</typeparam>
    public interface IBaseRepository<T>
        where T : class
    {
        Task<T> GetByIdAsync(int id);

        /// <summary>
        /// Gets all entities in the repository, optionally ordering them by the specified criteria.
        /// </summary>
        /// <param name="orderBy">A lambda expression specifying the property to order by.</param>
        /// <param name="orderByDirection">The direction to order the results in.</param>
        /// <returns>A Task representing the asynchronous operation that returns all entities in the repository.</returns>
        Task<IEnumerable<T>> GetAllAsync(
            Expression<Func<T, object>> orderBy = null,
            string orderByDirection = OrderBy.Ascending
        );

        /// <summary>
        /// Finds a single entity based on the provided criteria and includes related entities as specified.
        /// </summary>
        /// <param name="criteria">The criteria used to filter the entity.</param>
        /// <param name="includes">The related entities to include in the result set.</param>
        /// <returns>The entity that satisfies the provided criteria or null if no entity is found.</returns>
        Task<T> FindAsync(Expression<Func<T, bool>> criteria, string[] includes = null);



        /// <summary>
        /// Retrieves all entities that match the specified criteria, with optional sorting and pagination.
        /// </summary>
        /// <param name="criteria">The criteria to use for filtering the entities.</param>
        /// <param name="skip">The number of entities to skip. Null to skip no entities.</param>
        /// <param name="take">The maximum number of entities to retrieve. Null to retrieve all matching entities.</param>
        /// <param name="includes">The related entities to include in the query results. Null to exclude related entities.</param>
        /// <param name="orderBy">The property to use for sorting the entities. Null to skip sorting.</param>
        /// <param name="orderByDirection">The direction to sort the entities in. Defaults to ascending.</param>
        /// <returns>A collection of entities that match the specified criteria, sorted and paginated if requested.</returns>
        Task<IEnumerable<T>> FindAllAsync(
            Expression<Func<T, bool>> criteria,
            int? skip,
            int? take,
            string[] includes = null,
            Expression<Func<T, object>> orderBy = null,
            string orderByDirection = OrderBy.Ascending
        );

        /// <summary>
        /// Retrieves all entities that match the specified criteria, with optional sorting and pagination.
        /// </summary>
        /// <param name="criteria">The criteria to use for filtering the entities.</param>
        /// <param name="includes">The related entities to include in the query results. Null to exclude related entities.</param>
        /// <param name="orderBy">The property to use for sorting the entities. Null to skip sorting.</param>
        /// <param name="orderByDirection">The direction to sort the entities in. Defaults to ascending.</param>
        /// <returns>A collection of entities that match the specified criteria, sorted and paginated if requested.</returns>
        Task<IEnumerable<T>> FindAllAsync(
            Expression<Func<T, bool>> criteria,
            string[] includes = null,
            Expression<Func<T, object>> orderBy = null,
            string orderByDirection = OrderBy.Ascending
        );

        /// <summary>
        /// Retrieves all entities, with optional sorting and pagination.
        /// </summary>
        /// <param name="skip">The number of entities to skip. Null to skip no entities.</param>
        /// <param name="take">The maximum number of entities to retrieve. Null to retrieve all entities.</param>
        /// <param name="includes">The related entities to include in the query results. Null to exclude related entities.</param>
        /// <param name="orderBy">The property to use for sorting the entities. Null to skip sorting.</param>
        /// <param name="orderByDirection">The direction to sort the entities in. Defaults to ascending.</param>
        /// <returns>A collection of entities, sorted and paginated if requested.</returns>
        Task<IEnumerable<T>> FindAllAsync(
            int? skip,
            int? take,
            string[] includes = null,
            Expression<Func<T, object>> orderBy = null,
            string orderByDirection = OrderBy.Ascending
        );
        Task<T> AddAsync(T entity);
        Task AddRangeAsync(IEnumerable<T> entities);
        Task<T> UpdateAsync(T entity);
        Task DeleteAsync(T entities);
        Task SoftDeleteAsync(T entity);
        Task DeleteRangeAsync(IEnumerable<T> entities);
        Task DeleteRangeAsync(Expression<Func<T, bool>> criteria);
        Task<bool> ExistsAsync(Expression<Func<T, bool>> criteria);
        Task<int> CountAsync();
        Task<int> CountAsync(Expression<Func<T, bool>> criteria);
    }
}
