using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TatumBackendApi.Common.Models;
using TatumBackendApi.Services;

namespace TatumBackendApi.Controllers
{
    [ApiController]
    [Route("api/Products")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet("{productId:guid}/items")]
        [Authorize]
        public async Task<IActionResult> GetProductItems(
            Guid productId,
            [FromQuery] PaginationParameters pagination,
            CancellationToken ct
        )
        {
            var response = await _productService.GetProductItemsAsync(
                productId,
                pagination,
                ct
            );

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }
    }
}