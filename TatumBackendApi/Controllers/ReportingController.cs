using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TatumBackendApi.DTOs;
using TatumBackendApi.Services;

namespace TatumBackendApi.Controllers
{
    [ApiController]
    [Route("api/Reporting")]
    public class ReportingController : ControllerBase
    {
        private readonly IReportingService _reportingService;

        public ReportingController(IReportingService reportingService)
        {
            _reportingService = reportingService;
        }

        [HttpGet("admin/summary")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAdminSummary( [FromQuery] AdminSummaryRequestDto request, CancellationToken ct)
        {
            var response = await _reportingService.GetAdminSummaryAsync(request, ct);
            if (!response.Success)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
    }
}