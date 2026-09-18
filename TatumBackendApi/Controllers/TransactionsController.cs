using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TatumBackendApi.Common.Models;
using TatumBackendApi.DTOs;
using TatumBackendApi.Services;

namespace TatumBackendApi.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class TransactionsController : ControllerBase
    {
        private readonly ITransactionService _service;
        private readonly ICurrentUserService _currentUserService;

        public TransactionsController(ITransactionService service, ICurrentUserService currentUserService)
        {
            _service = service;
            _currentUserService = currentUserService;

        }

        [HttpPost("purchase")]
        public async Task<IActionResult> Purchase(
            [FromBody]
            ProductPurchaseRequestDto request,
            CancellationToken ct)
        {
            var response =
                await _service.PurchaseAsync(
                    request,
                    ct);

            if (!response.Success)
            {
                if (response.Errors?.Any(
                    x => x.Code == "AccountNotFound") == true)
                    return NotFound(response);

                if (response.Errors?.Any(
                    x => x.Code == "ProductNotFound") == true)
                    return NotFound(response);

                if (response.Errors?.Any(
                    x => x.Code == "ProductItemNotFound") == true)
                    return NotFound(response);

                if (response.Errors?.Any(
                    x => x.Code == "InsufficientBalance") == true)
                    return BadRequest(response);

                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> GetTransactions(
            [FromQuery] PaginationParameters pagination,
            [FromQuery] TransactionFilterDto filter)
        {
            var isAdmin = _currentUserService.IsAdmin || _currentUserService.IsSuperAdmin;



            if (!isAdmin)
            {
               if(_currentUserService.UserId == Guid.Empty)
                {
                    return Unauthorized();
                }

               filter.UserId = _currentUserService.UserId;
            }

            var response = await _service.GetTransactionsAsync(pagination, filter);

            return response.Success ? Ok(response) : BadRequest(response);
        }
    }
}