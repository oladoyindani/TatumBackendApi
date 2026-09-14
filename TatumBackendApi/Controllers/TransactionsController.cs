using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TatumBackendApi.DTOs;
using TatumBackendApi.Services;

namespace TatumBackendApi.Controllers
{
    public class TransactionsController : ControllerBase
    {
        private readonly ITransactionService _service;

        public TransactionsController(ITransactionService service)
        {
            _service = service;
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
    }
}