using Microsoft.EntityFrameworkCore;
using TatumBackendApi.Common.Models;
using TatumBackendApi.Data;
using TatumBackendApi.Entities;

namespace TatumBackendApi.Repositories
{
    public class ProductItemRepository : IProductItemRepository
    {
        private readonly AppDbContext _context;
        public ProductItemRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<PagedResult<ProductItem>> GetByProductIdAsync(
            Guid productId,
            PaginationParameters pagination,
            CancellationToken ct = default
        )
        {
            var pageNumber = pagination.PageNumber < 1 ? 1 : pagination.PageNumber;
            var query = _context.ProductItems.AsNoTracking().Where(item => item.ProductId == productId);
            var totalCount = await query.CountAsync(ct);
            var totalPages = (int)Math.Ceiling(totalCount / (double)pagination.PageSize);
            var items = await query
                .OrderByDescending(item => item.CreatedAt)
                .Skip((pageNumber - 1) * pagination.PageSize)
                .Take(pagination.PageSize)
                .ToListAsync(ct);

            return new PagedResult<ProductItem>
            {
                Items = items,
                PageNumber = pageNumber,
                PageSize = pagination.PageSize,
                TotalCount = totalCount,
                TotalPages = totalPages
            };
        }
    }
}