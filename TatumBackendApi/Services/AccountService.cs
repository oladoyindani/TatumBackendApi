using TatumBackendApi.Common.Models;
using TatumBackendApi.DTOs;
using TatumBackendApi.Entities;
using TatumBackendApi.Repositories;

namespace TatumBackendApi.Services
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _accountRepository;

        public AccountService(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        public async Task<List<AccountDto>> GetMyAccountsAsync(Guid customerId, CancellationToken ct = default)
        {
            var accounts = await _accountRepository.GetFilteredAsync(customerId: customerId);

            return accounts.Select(MapToDto).ToList();
        }

        public async Task<AccountDto?> GetMyAccountByIdAsync(
               Guid customerId,
               Guid accountId,
               CancellationToken ct = default)
        {
            var accounts = await _accountRepository.GetFilteredAsync(id: accountId, customerId: customerId);

            var account = accounts.FirstOrDefault();

            return account is null ? null : MapToDto(account);
        }

        public async Task<PagedResult<AccountDto>> GetAllAccountsAsync(
            Guid? id,
            Guid? customerId,
            string? accountNumber,
            PaginationParameters pagination,
            CancellationToken ct = default)
        {
            var result = await _accountRepository.GetPagedAsync(
                id, customerId, accountNumber, pagination, ct);

            return new PagedResult<AccountDto>
            {
                Items = result.Items.Select(MapToDto).ToList(),
                PageNumber = result.PageNumber,
                PageSize = result.PageSize,
                TotalCount = result.TotalCount,
                TotalPages = result.TotalPages,
            };
        }

        private static AccountDto MapToDto(Account account)
        {
            return new AccountDto
            {
                Id = account.Id,
                AccountNumber = account.AccountNumber,
                Name = account.Name,
                Currency = account.Currency,
                LedgerBalance = account.LedgerBalance,
                AvailableBalance = account.AvailableBalance,
                Status = account.Status,
                CreatedAt = account.CreatedAt,
            };
        }
    }
}
