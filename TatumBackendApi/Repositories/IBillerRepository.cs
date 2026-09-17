using TatumBackendApi.Entities;
using TatumBackendApi.Common.Models;

namespace TatumBackendApi.Repositories;

public interface IBillerRepository
{
    Task<PagedResult<Biller>> GetPagedAsync(
        BillerCategory? category,
        PaginationParameters pagination,
        CancellationToken ct = default);

    Task<Biller?> GetByIdAsync(Guid id, CancellationToken ct = default);
    
    Task<bool> ExistsAsync(Guid billerId, CancellationToken ct = default);
}
