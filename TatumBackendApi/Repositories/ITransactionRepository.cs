using TatumBackendApi.Common.Models;
using TatumBackendApi.DTOs;
using TatumBackendApi.Entities;

namespace TatumBackendApi.Repositories
{
    public interface ITransactionRepository : IRepository<Transaction>
    {
        Task<PagedResult<Transaction>> GetPagedAsync(
            TransactionFilterDto filter,
            PaginationParameters pagination,
            CancellationToken ct = default);
    }
}
