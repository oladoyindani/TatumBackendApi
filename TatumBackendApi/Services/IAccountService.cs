using TatumBackendApi.Common.Models;
using TatumBackendApi.DTOs;

namespace TatumBackendApi.Services
{
    public interface IAccountService
    {
        Task<List<AccountDto>> GetMyAccountsAsync(Guid customerId, CancellationToken ct = default);

        Task<AccountDto?> GetMyAccountByIdAsync(Guid CustomerId, Guid accountId, CancellationToken ct = default);

        Task<PagedResult<AccountDto>> GetAllAccountsAsync(Guid? id, Guid? customerId, string? accountNumber, PaginationParameters pagination, CancellationToken ct = default);
    }
}
