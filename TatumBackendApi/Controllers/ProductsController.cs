using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TatumBackendApi.Common.Models;
using TatumBackendApi.DTOs;
using TatumBackendApi.Services;
namespace TatumBackendApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IBillerService _service;
    private readonly IProductService _productService;

    public ProductsController(IBillerService service, IProductService productService)
    {
        _service = service;
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

    [HttpGet("billers")]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<BillerDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<PagedResult<BillerDto>>>> GetBillers(
        [FromQuery] BillerQueryParameters query,
        CancellationToken ct)
    {
        return Ok(await _service.GetAsync(query, ct));
    }

    [HttpGet("billers/{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<BillerDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<BillerDto>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<BillerDto>>> GetBiller(
        Guid id,
        CancellationToken ct)
    {
        var response = await _service.GetByIdAsync(id, ct);
        return response.Success ? Ok(response) : NotFound(response);
    }
}
