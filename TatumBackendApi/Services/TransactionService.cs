using TatumBackendApi.DTOs;
using TatumBackendApi.Entities;
using TatumBackendApi.Common.Models;
using TatumBackendApi.Repositories;
using TatumBackendApi.Responses;

namespace TatumBackendApi.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly ITransactionRepository _transactionRepository;

        public TransactionService(ITransactionRepository transactionRepository)
        {
            _transactionRepository = transactionRepository;
        }

        public async Task<ApiResponse<PagedResult<TransactionDto>>> GetTransactionsAsync(
            PaginationParameters pagination,
            TransactionFilterDto? filter = null)
        {
            filter ??= new TransactionFilterDto();

            var result = await _transactionRepository.GetPagedAsync(filter, pagination);

            var dtoResult = new PagedResult<TransactionDto>
            {
                Items = result.Items.Select(MapToDto).ToList(),
                PageNumber = result.PageNumber,
                PageSize = result.PageSize,
                TotalCount = result.TotalCount,
                TotalPages = result.TotalPages
            };

            return ApiResponse<PagedResult<TransactionDto>>.Ok(dtoResult);
        }

        public async Task<ApiResponse<TransactionResponseDto>> PurchaseAsync(
        ProductPurchaseRequestDto request,
        CancellationToken ct = default)
            {
                throw new NotImplementedException("Purchase flow not yet built.");
            }

        private static TransactionDto MapToDto(Transaction transaction)
        {
            return new TransactionDto
            {
                Id = transaction.Id,
                UserId = transaction.CustomerId,
                UserName = transaction.User is null
                    ? null
                    : $"{transaction.User.FirstName} {transaction.User.LastName}".Trim(),
                AccountId = transaction.AccountId,
                AccountNumber = transaction.Account?.AccountNumber,
                BillerId = transaction.BillerId,
                ProductId = transaction.ProductId,
                ProductCategory = transaction.Product?.Category.ToString(),
                Amount = transaction.Amount,
                Status = transaction.Status,
                Reference = transaction.Reference,
                Description = transaction.Narration,
                CreatedAt = transaction.CreatedAt,
                CompletedAt = transaction.CompletedAt
            };
        }
    }
}