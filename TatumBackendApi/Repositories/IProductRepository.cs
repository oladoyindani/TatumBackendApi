using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TatumBackendApi.Entities;

namespace TatumBackendApi.Repositories
{
    public interface IProductRepository
    {
        Task<(List<Product> Items, int TotalCount)> GetByBillerIdAsync(
            Guid billerId,
            ProductCategory? category,
            int pageNumber,
            int pageSize,
            CancellationToken ct = default);
    }
}