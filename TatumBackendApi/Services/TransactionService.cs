using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using TatumBackendApi.Common.Models;
using TatumBackendApi.Data;
using TatumBackendApi.DTOs;
using TatumBackendApi.Entities;
using TatumBackendApi.Repositories;
using TatumBackendApi.Responses;

namespace TatumBackendApi.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly AppDbContext _context;

        public TransactionService(
            ITransactionRepository transactionRepository,
            AppDbContext context)
        {
            _transactionRepository = transactionRepository;
            _context = context;
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
            if (request == null)
            {
                return ApiResponse<TransactionResponseDto>.Fail(
                    "Purchase request is required.",
                    new List<ApiError> { new("InvalidRequest", "Purchase request is required.") });
            }

            if (request.AccountId == Guid.Empty)
            {
                return ApiResponse<TransactionResponseDto>.Fail(
                    "Account is required.",
                    new List<ApiError> { new("AccountNotFound", "A valid account is required.") });
            }

            if (request.ProductId == Guid.Empty)
            {
                return ApiResponse<TransactionResponseDto>.Fail(
                    "Product is required.",
                    new List<ApiError> { new("ProductNotFound", "A valid product is required.") });
            }

            var account = await _context.Accounts
                .FirstOrDefaultAsync(a => a.Id == request.AccountId && a.IsActive, ct);

            if (account == null)
            {
                return ApiResponse<TransactionResponseDto>.Fail(
                    "Account not found.",
                    new List<ApiError> { new("AccountNotFound", "No active account was found for this id.") });
            }

            var product = await _context.Products
                .Include(p => p.ProductItems)
                .FirstOrDefaultAsync(p => p.Id == request.ProductId && p.IsActive, ct);

            if (product == null)
            {
                return ApiResponse<TransactionResponseDto>.Fail(
                    "Product not found.",
                    new List<ApiError> { new("ProductNotFound", "No active product was found for this id.") });
            }

            ProductItem? productItem = null;
            if (request.ProductItemId.HasValue)
            {
                productItem = await _context.ProductItems
                    .FirstOrDefaultAsync(
                        pi => pi.Id == request.ProductItemId.Value
                            && pi.ProductId == product.Id
                            && pi.IsActive,
                        ct);

                if (productItem == null)
                {
                    return ApiResponse<TransactionResponseDto>.Fail(
                        "Product item not found.",
                        new List<ApiError> { new("ProductItemNotFound", "The selected product item was not found.") });
                }
            }
            else if (product.RequiresProductItem)
            {
                return ApiResponse<TransactionResponseDto>.Fail(
                    "Product item is required.",
                    new List<ApiError> { new("ProductItemNotFound", "This product requires a product item selection.") });
            }

            var amount = productItem?.UnitPrice ?? request.Amount ?? product.UnitPrice ?? 0m;

            if (amount <= 0m)
            {
                return ApiResponse<TransactionResponseDto>.Fail(
                    "Amount must be greater than zero.",
                    new List<ApiError> { new("InvalidAmount", "A valid purchase amount is required.") });
            }

            if (product.AllowsCustomAmount && request.Amount is null)
            {
                return ApiResponse<TransactionResponseDto>.Fail(
                    "Amount is required for this product.",
                    new List<ApiError> { new("InvalidAmount", "This product requires a custom amount.") });
            }

            if (product.AllowsCustomAmount && request.Amount.HasValue && request.Amount.Value <= 0m)
            {
                return ApiResponse<TransactionResponseDto>.Fail(
                    "Amount must be greater than zero.",
                    new List<ApiError> { new("InvalidAmount", "The provided amount is invalid.") });
            }

            if (account.AvailableBalance < amount)
            {
                return ApiResponse<TransactionResponseDto>.Fail(
                    "Insufficient balance.",
                    new List<ApiError> { new("InsufficientBalance", "The selected account does not have enough available balance.") });
            }

            var reference = $"TXN-{DateTime.UtcNow:yyyyMMddHHmmss}-{Guid.NewGuid().ToString("N")[..8]}";
            var status = TatumBackendApi.Common.Constants.Transaction.TransactionStatus.Pending.ToString();

            var transaction = new Transaction
            {
                Id = Guid.NewGuid(),
                AccountId = account.Id,
                CustomerId = account.CustomerId,
                Type = product.Category.ToString(),
                Status = status,
                Reference = reference,
                Amount = amount,
                Currency = account.Currency,
                BillerId = product.BillerId,
                ProductId = product.Id,
                ProductItemId = productItem?.Id,
                Narration = product.Name,
                CustomerFields = request.Fields.Count > 0
                    ? JsonSerializer.Serialize(request.Fields)
                    : null,
                CreatedAt = DateTime.UtcNow,
                CompletedAt = null
            };

            account.AvailableBalance -= amount;
            account.LedgerBalance -= amount;

            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync(ct);

            var response = new TransactionResponseDto
            {
                Id = transaction.Id,
                Reference = transaction.Reference,
                Type = transaction.Type,
                Status = transaction.Status,
                Amount = transaction.Amount,
                Currency = transaction.Currency,
                BillerName = product.Biller?.Name,
                ProductName = product.Name,
                ProductItemName = productItem?.Name,
                CreatedAt = transaction.CreatedAt,
                CompletedAt = transaction.CompletedAt
            };

            return ApiResponse<TransactionResponseDto>.Ok(
                response,
                "Purchase completed successfully.");
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