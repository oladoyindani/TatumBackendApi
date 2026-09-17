using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.Mvc;
using TatumBackendApi.Dtos;
using TatumBackendApi.DTOs;
using TatumBackendApi.Services;


namespace TatumBackendApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _auth;
        
        public AuthController(IAuthService auth)
        {
            _auth = auth;
        }

        //CUSTOMER REGISTRATION

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto request, CancellationToken ct)
        {
            try
            {
                var response = await _auth.RegisterAsync(request, ct);

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

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request, CancellationToken ct)
        {
            try
            {
                var response = await _auth.LoginAsync(request, ct);
                if (!response.Success)
                {
                    return Conflict(response);
                }

                return Ok(response);
            } catch (Exception ex)
            {
                Console.WriteLine(ex);
                return BadRequest("An error occured");
            }

        }

        [HttpPost("verify-registration-otp")]
        [AllowAnonymous]
        public async Task<IActionResult> VerifyRegistrationOtp([FromBody] VerifyRegistrationOtpRequestDto request, CancellationToken ct)
        {
            try
            {
                var response = await _auth.VerifyRegistrationOtpAsync(request, ct);
                if(!response.Success)
                {
                    return Conflict(response); 
                }
                return Ok(response);

            }
            catch (Exception)
            {
                return BadRequest("There's an Error");
            }
        }

        [HttpPost("reset-password-start")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPasswordStart([FromBody] ResetPasswordStartDto request, CancellationToken ct)
        {
            try
            {
                var response = await _auth.ResetPasswordStartAsync(request, ct);

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


        [HttpPost("reset-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto request, CancellationToken ct)
        {
            try
            {
                var response = await _auth.ResetPasswordAsync(request, ct);

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

        [Authorize]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword(
            [FromBody] ChangePasswordRequestDto request,
            CancellationToken ct)
        {
            try
            {
                var userId = GetCurrentUserId();
                var response = await _auth.ChangePasswordAsync(userId, request, ct);
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
        
        [HttpPost("resend-registration-otp")]
        [AllowAnonymous]
        public async Task<IActionResult> ResendRegistrationOtp([FromBody] ResendRegistrationOtpRequestDto request, CancellationToken ct)
        {
            try
            {
                var response = await _auth.ResendRegistrationOtpAsync(request, ct);
                if (!response.Success)
                {
                    return BadRequest(response);
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return BadRequest("An error occured");
            }
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> Me(CancellationToken ct)
        {
            try
            {
                var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier)
                    ?? User.FindFirstValue("sub");

                if (string.IsNullOrWhiteSpace(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
                {
                    return Unauthorized("Invalid or missing user identifier in token.");
                }

                var response = await _auth.GetCurrentUserAsync(userId, ct);
                if (!response.Success)
                {
                    return NotFound(response);
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return BadRequest("An error occured");
            }
        }

        [HttpPost("admin/invite")]
        [Authorize(Roles = Common.Constants.UserRoles.SuperAdmin)]
        public async Task<IActionResult> AdminInvite([FromBody] AdminInviteRequestDto request, CancellationToken ct)
        {
            try
            {
                var response = await _auth.InviteAdminAsync(request, ct);
                if (!response.Success) return Conflict(response);
                return Ok(response);
            }
            catch (Exception ex) { Console.WriteLine(ex); return BadRequest("An error occured"); }
        }


        [HttpPost("set-password")]
        [AllowAnonymous]
        public async Task<IActionResult> SetPassword([FromBody] SetPasswordRequestDto request, CancellationToken ct)
        {
            try
            {
                var response = await _auth.SetPasswordAsync(request, ct);
                if (!response.Success)
                {
                    return Unauthorized(response);
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return BadRequest("An error occured");
            }
        }
    }
}