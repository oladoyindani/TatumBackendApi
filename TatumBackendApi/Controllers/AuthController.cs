using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.Mvc;
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
            catch (Exception ex)
            {
                return BadRequest("There's an Error");
            }
        }
    }
}