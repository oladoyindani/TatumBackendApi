using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TatumBackendApi.Common.Models;
using TatumBackendApi.DTOs;
using TatumBackendApi.Repositories;

namespace TatumBackendApi.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductItemRepository _productItemRepository;

        public ProductService(
            IProductItemRepository productItemRepository
        )
        {
            _productItemRepository = productItemRepository;
        }

        public async Task<ApiResponse<PagedResult<ProductItemDto>>>
            GetProductItemsAsync(
                Guid productId,
                PaginationParameters pagination,
                CancellationToken ct = default
            )
        {
            var result = await _productItemRepository
                .GetByProductIdAsync(productId, pagination, ct);

            var response = new PagedResult<ProductItemDto>
            {
                PageNumber = result.PageNumber,
                PageSize = result.PageSize,
                TotalCount = result.TotalCount,
                TotalPages = result.TotalPages,
                Items = result.Items
                    .Select(item => new ProductItemDto
                    {
                        Id = item.Id,
                        ProductId = item.ProductId,
                        Name = item.Name,
                        Description = item.Description,
                        // Price = item.Price,
                        // Quantity = item.Quantity,
                        // IsActive = item.IsActive
                    })
                    .ToList()
            };

            return ApiResponse<PagedResult<ProductItemDto>>.Ok(
                response,
                "Product items retrieved successfully."
            );
        }
    }
}