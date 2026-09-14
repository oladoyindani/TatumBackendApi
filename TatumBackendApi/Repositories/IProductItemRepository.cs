using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TatumBackendApi.Common.Models;
using TatumBackendApi.Entities;

namespace TatumBackendApi.Repositories
{
    public interface IProductItemRepository
    {
        Task<PagedResult<ProductItem>> GetByProductIdAsync(
            Guid productId,
            PaginationParameters pagination,
            CancellationToken ct = default
        );
    }
}