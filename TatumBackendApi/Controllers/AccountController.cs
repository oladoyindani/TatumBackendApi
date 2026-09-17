using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TatumBackendApi.Common.Constants;
using TatumBackendApi.Common.Models;
using TatumBackendApi.Services;

namespace TatumBackendApi.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly ICurrentUserService _currentUserService;

        public AccountController(
            IAccountService accountService,
            ICurrentUserService currentUserService)
        {
            _accountService = accountService;
            _currentUserService = currentUserService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAccounts(
            [FromQuery] Guid? id,
            [FromQuery] Guid? CustomerId,
            [FromQuery] string? AccountNumber,
            [FromQuery] PaginationParameters Pagination,
            CancellationToken ct)
        {
            var currentUserId = _currentUserService.UserId;

            if (currentUserId == Guid.Empty)
            {
                return Unauthorized();
            }

            var role = _currentUserService.Role ?? string.Empty;
            var isAdmin = role == UserRoles.Admin || role == UserRoles.SuperAdmin;

            if (isAdmin)
            {
                var result = await _accountService.GetAllAccountsAsync(
                    id, CustomerId, AccountNumber, Pagination, ct);
                return Ok(result);
            }

            var accounts = await _accountService.GetMyAccountsAsync(currentUserId, ct);

            return Ok(accounts);
        }
    }
}