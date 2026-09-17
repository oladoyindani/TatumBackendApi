using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TatumBackendApi.Common.Models;
using TatumBackendApi.DTOs;
using TatumBackendApi.Entities;
using TatumBackendApi.Repositories;
using TatumBackendApi.Responses;

namespace TatumBackendApi.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductItemRepository _productItemRepository;
        private readonly IBillerRepository _billerRepository;
        private readonly IProductRepository _productRepository;



        public ProductService(
            IProductRepository productRepository,
            IProductItemRepository productItemRepository,
            IBillerRepository billerRepository
        )
        {
            _productRepository = productRepository;
            _productItemRepository = productItemRepository;
            _billerRepository = billerRepository;
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

        public async Task<ApiResponse<PagedResult<ProductDto>>> GetProductsByBillerIdAsync(
            Guid billerId,
            ProductCategory? category,
            PaginationParameters pagination,
            CancellationToken ct = default)
        {
            // ===============================
            // NORMALIZE PAGE NUMBER
            // ===============================
            // PaginationParameters already clamps PageSize via its setter,
            // but PageNumber has no lower bound guard, so we enforce it here.

            if (pagination.PageNumber < 1)
            {
                pagination.PageNumber = 1;
            }

            // ===============================
            // CHECK BILLER EXISTS
            // ===============================

            var billerExists = await _billerRepository.ExistsAsync(billerId, ct);

            if (!billerExists)
            {
                return ApiResponse<PagedResult<ProductDto>>.Fail(
                    "Biller not found.",
                    new List<ApiError>
                    {
                        new("BillerNotFound", "No biller was found with the given id.")
                    });
            }

            // ===============================
            // FETCH PRODUCTS
            // ===============================

            var (items, totalCount) = await _productRepository.GetByBillerIdAsync(
                billerId,
                category,
                pagination.PageNumber,
                pagination.PageSize,
                ct);

            var totalPages = pagination.PageSize == 0
                ? 0
                : (int)Math.Ceiling(totalCount / (double)pagination.PageSize);

            var result = new PagedResult<ProductDto>
            {
                Items = items.Select(MapToDto).ToList(),
                PageNumber = pagination.PageNumber,
                PageSize = pagination.PageSize,
                TotalCount = totalCount,
                TotalPages = totalPages
            };

            // ===============================
            // RESPONSE
            // ===============================

            return ApiResponse<PagedResult<ProductDto>>.Ok(
                result,
                "Products retrieved successfully.");
        }

        private static ProductDto MapToDto(Product product)
        {
            return new ProductDto
            {
                Id = product.Id,
                Code = product.Code,
                Name = product.Name,
                Category = product.Category.ToString(),
                Description = product.Description,
                UnitPrice = product.UnitPrice,
                AllowsCustomAmount = product.AllowsCustomAmount,
                RequiresProductItem = product.RequiresProductItem,
                RequiredFields = product.RequiredFields,
                ProductItems = product.ProductItems
                    .Select(i => new ProductItemDto
                    {
                        Id = i.Id,
                        Code = i.Code,
                        Name = i.Name,
                        UnitPrice = i.UnitPrice,
                        Description = i.Description
                    })
                    .ToList()
            };
        }
    }
}