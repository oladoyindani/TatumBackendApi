using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TatumBackendApi.Common.Models;
using TatumBackendApi.DTOs;


namespace TatumBackendApi.Services
{
    public interface IProductService
    {
        Task<ApiResponse<PagedResult<ProductItemDto>>> GetProductItemsAsync(
            Guid productId,
            PaginationParameters pagination,
            CancellationToken ct = default
        );
    }
}