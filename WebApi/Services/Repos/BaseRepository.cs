using Data;
using Microsoft.EntityFrameworkCore;
using Models.Core;
using System.Linq.Expressions;
using OrderBy = Models.Consts.OrderBy;

namespace Services.Repos
{
    public class BaseRepository<T> : IBaseRepository<T>
        where T : class
    {
        private readonly ApplicationDbContext _context;

        public BaseRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<T>> GetAllAsync(
            Expression<Func<T, object>> orderBy = null,
            string orderByDirection = OrderBy.Ascending
        )
        {
            IQueryable<T> query = _context.Set<T>();

            if (orderBy != null)
            {
                if (orderByDirection == OrderBy.Ascending)
                    query = query.OrderBy(orderBy);
                else
                    query = query.OrderByDescending(orderBy);
            }
            return await query.ToListAsync();
        }

        public async Task<T> GetByIdAsync(int id)
        {
            return await _context.Set<T>().FindAsync(id);
        }

        public async Task<T> FindAsync(Expression<Func<T, bool>> criteria, string[] includes = null)
        {
            IQueryable<T> query = _context.Set<T>();
            

            query = query.Where(criteria);

            if (includes != null)
            {
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
            }

            return await query.FirstOrDefaultAsync(criteria);
        }

        public async Task<IEnumerable<T>> FindAllAsync(
            Expression<Func<T, bool>> criteria,
            int? take,
            int? skip,
            string[] includes = null,
            Expression<Func<T, object>> orderBy = null,
            string orderByDirection = OrderBy.Ascending
        )
        {
            IQueryable<T> query = _context.Set<T>();

            if (take.HasValue)
                query = query.Take(take.Value);

            if (skip.HasValue)
                query = query.Skip(skip.Value);

            if (orderBy != null)
            {
                if (orderByDirection == OrderBy.Ascending)
                    query = query.OrderBy(orderBy);
                else
                    query = query.OrderByDescending(orderBy);
            }

            if (includes != null)
                foreach (var include in includes)
                    query = query.Include(include);

            return await query.Where(criteria).ToListAsync();
        }

        public async Task<IEnumerable<T>> FindAllAsync(
            Expression<Func<T, bool>> criteria,
            string[] includes = null,
            Expression<Func<T, object>> orderBy = null,
            string orderByDirection = OrderBy.Ascending
        )
        {
            IQueryable<T> query = _context.Set<T>();

            if (orderBy != null)
            {
                if (orderByDirection == OrderBy.Ascending)
                    query = query.OrderBy(orderBy);
                else
                    query = query.OrderByDescending(orderBy);
            }

            if (includes != null)
                foreach (var include in includes)
                    query = query.Include(include);

            return await query.Where(criteria).ToListAsync();
        }

        public async Task<IEnumerable<T>> FindAllAsync(
            int? skip,
            int? take,
            string[] includes = null,
            Expression<Func<T, object>> orderBy = null,
            string orderByDirection = OrderBy.Ascending
        )
        {
            IQueryable<T> query = _context.Set<T>();

            // Order the query by the specified property and direction
            if (orderBy != null)
            {
                if (orderByDirection == OrderBy.Ascending)
                    query = query.OrderBy(orderBy);
                else
                    query = query.OrderByDescending(orderBy);
            }

            // Apply the skip and take options
            if (take.HasValue)
                query = query.Take(take.Value);

            if (skip.HasValue)
                query = query.Skip(skip.Value);

            // Include related entities if specified
            if (includes != null)
                foreach (var include in includes)
                    query = query.Include(include);

            // Execute the query and return the results as an IEnumerable
            return await query.ToListAsync();
        }

        public async Task<T> AddAsync(T entity)
        {
            await _context.Set<T>().AddAsync(entity);
            _context.SaveChanges();
            return entity;
        }

        public async Task AddRangeAsync(IEnumerable<T> entities)
        {
            await _context.Set<T>().AddRangeAsync(entities);
            _context.SaveChanges();
        }

        public async Task<T> UpdateAsync(T entity)
        {
            _context.Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task DeleteAsync(T entity)
        {
            _context.Set<T>().Remove(entity);
            await _context.SaveChangesAsync();
        }


   

        public async Task DeleteRangeAsync(IEnumerable<T> entities)
        {
            _context.Set<T>().RemoveRange(entities);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteRangeAsync(Expression<Func<T, bool>> criteria)
        {
            var entities = _context.Set<T>().Where(criteria);
            _context.Set<T>().RemoveRange(entities);
            await _context.SaveChangesAsync();
        }


        public async Task SoftDeleteAsync(T entity)
        {
            if (entity is SoftDeletable softDeletableEntity)
            {
                softDeletableEntity.IsDeleted = true; 
                _context.Update(entity);
                await _context.SaveChangesAsync(); 
            }
        }

        public async Task<int> CountAsync()
        {
            return await _context.Set<T>().CountAsync();
        }

        public async Task<int> CountAsync(Expression<Func<T, bool>> criteria)
        {
            return await _context.Set<T>().CountAsync(criteria);
        }

        public async Task<bool> ExistsAsync(Expression<Func<T, bool>> criteria)
        {
            return await _context.Set<T>().AnyAsync(criteria);
        }

      
    }
}
