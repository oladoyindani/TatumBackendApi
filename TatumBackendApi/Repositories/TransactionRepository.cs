using TatumBackendApi.Data;
using TatumBackendApi.DTOs;
using TatumBackendApi.Entities;
using TatumBackendApi.Common.Models;
using Microsoft.EntityFrameworkCore;

namespace TatumBackendApi.Repositories
{
    public class TransactionRepository : Repository<Transaction>, ITransactionRepository
    {
        public TransactionRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<PagedResult<Transaction>> GetPagedAsync(
            TransactionFilterDto filter,
            PaginationParameters pagination,
            CancellationToken ct = default)
        {
            var query = _context.Transactions
                .AsNoTracking()
                .Include(t => t.User)
                .Include(t => t.Account)
                .AsQueryable();

            if (filter.TransactionId.HasValue)
            {
                query = query.Where(t => t.Id == filter.TransactionId.Value);
            }

            if (filter.UserId.HasValue)
            {
                query = query.Where(t => t.CustomerId == filter.UserId.Value);
            }

            if (filter.AccountId.HasValue)
            {
                query = query.Where(t => t.AccountId == filter.AccountId.Value);
            }

            if (!string.IsNullOrWhiteSpace(filter.AccountNumber))
            {
                var number = filter.AccountNumber.Trim();
                query = query.Where(t => t.Account.AccountNumber == number);
            }

            if (filter.BillerId.HasValue)
            {
                query = query.Where(t => t.BillerId == filter.BillerId.Value);
            }

            if (filter.ProductId.HasValue)
            {
                query = query.Where(t => t.ProductId == filter.ProductId.Value);
            }
            // REMEMBER TO UNCOMMENT THIS CODE
            // if (filter.Status.HasValue)
            // {
            //     query = query.Where(t => t.Status == filter.Status.Value);
            // }

            //if (filter.Category.HasValue)
            //{
            //    query = query.Where(t => t.Category == filter.Category.Value);
            //}

            if (filter.FromDate.HasValue)
            {
                query = query.Where(t => t.CreatedAt >= filter.FromDate.Value);
            }

            if (filter.ToDate.HasValue)
            {
                query = query.Where(t => t.CreatedAt <= filter.ToDate.Value);
            }

            var totalCount = await query.CountAsync(ct);

            var totalPages = (int)Math.Ceiling(totalCount / (double)pagination.PageSize);

            var items = await query
                .OrderByDescending(t => t.CreatedAt)
                .Skip((pagination.PageNumber - 1) * pagination.PageSize)
                .Take(pagination.PageSize)
                .ToListAsync(ct);

            return new PagedResult<Transaction>
            {
                Items = items,
                PageNumber = pagination.PageNumber,
                PageSize = pagination.PageSize,
                TotalCount = totalCount,
                TotalPages = totalPages
            };
        }
    }
}