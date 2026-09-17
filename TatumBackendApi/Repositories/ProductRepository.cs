using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TatumBackendApi.Data;
using TatumBackendApi.Entities;

namespace TatumBackendApi.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly AppDbContext _context;

        public ProductRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<(List<Product> Items, int TotalCount)> GetByBillerIdAsync(
            Guid billerId,
            ProductCategory? category,
            int pageNumber,
            int pageSize,
            CancellationToken ct = default)
        {
            var query = _context.Products
                .Where(p => p.BillerId == billerId && p.IsActive)
                .AsQueryable();

            // ===============================
            // OPTIONAL CATEGORY FILTER
            // ===============================

            if (category.HasValue)
            {
                query = query.Where(p => p.Category == category.Value);
            }

            // ===============================
            // TOTAL COUNT (before paging)
            // ===============================

            var totalCount = await query.CountAsync(ct);

            // ===============================
            // APPLY PAGING
            // ===============================

            var items = await query
                .OrderBy(p => p.Name)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Include(p => p.ProductItems.Where(i => i.IsActive))
                .AsNoTracking()
                .ToListAsync(ct);

            return (items, totalCount);
        }
    }
}