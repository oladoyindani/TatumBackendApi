using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using TatumBackendApi.Common.Constants;
using TatumBackendApi.DTOs;
using TatumBackendApi.Services;

namespace TatumBackendApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _user;

        public UsersController(IUserService user)
        {
            _user = user;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetUsers(
            [FromQuery] GetUsersQueryDto query,
            CancellationToken ct)
        {
            try
            {
                var userId = GetCurrentUserId();
                var isAdmin = User.IsInRole(UserRoles.Admin);
                var response = await _user.GetUsersAsync(query, userId, isAdmin, ct);
                if (!response.Success)
                {
                    return Conflict(response);
                }

                return Ok(response);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return BadRequest("An error occured");
            }
        }

        [HttpPut("profile")]
        [Authorize]
        public async Task<IActionResult> UpdateProfile(
            [FromBody] UpdateProfileRequestDto request,
            CancellationToken ct)
        {
            try
            {
                var userId = GetCurrentUserId();
                var response = await _user.UpdateProfileAsync(userId, request, ct);
                if (!response.Success)
                {
                    return Conflict(response);
                }

                return Ok(response);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return BadRequest("An error occured");
            }
        }

        private Guid GetCurrentUserId()
        {
            var idClaim = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                          ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            return Guid.TryParse(idClaim, out var id) ? id : throw new UnauthorizedAccessException();
        }
    }
}
