using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using TatumBackendApi.Common.Models;

namespace TatumBackendApi.Repositories
{
    public interface IRepository<T> where T : class
    {
        Task<T?> GetByIdAsync(Guid id);

        Task<IEnumerable<T>> GetAllAsync();

        Task AddAsync(T entity);

        void Update(T entity);

        Task UpdateAsync(T entity);
        
        void Delete(T entity);

        Task DeleteAsync(T entity);

        Task<int> CountAsync( Expression<Func<T, bool>> predicate);

        Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);

        // PAGINATION

        Task<PagedResult<T>> GetPagedAsync(
            int pageNumber = 1,
            int pageSize = 10,
            CancellationToken ct = default
        );

        Task<PagedResult<T>> GetPagedAsync(
            Expression<Func<T, bool>> predicate,
            int pageNumber = 1,
            int pageSize = 10,
            CancellationToken ct = default
        );

        Task<int> SaveChangesAsync(CancellationToken ct = default);
    }

    
}