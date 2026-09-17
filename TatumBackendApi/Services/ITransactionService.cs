using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TatumBackendApi.Common.Models;
using TatumBackendApi.DTOs;

namespace TatumBackendApi.Services
{
    public interface ITransactionService
    {
        Task<ApiResponse<TransactionResponseDto>>
        PurchaseAsync(
            ProductPurchaseRequestDto request,
            CancellationToken ct = default);

        Task<ApiResponse<PagedResult<TransactionDto>>>
            GetTransactionsAsync(
                PaginationParameters pagination,
                TransactionFilterDto? filter = null);

        // Task<ApiResponse<TransactionDto>>
        //     GetTransactionByIdAsync(Guid id);

        // Task<ApiResponse<TransactionSummaryDto>>
        //     GetAdminTransactionSummaryAsync(
        //         TransactionSummaryFilterDto filter);

    }
}