using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TatumBackendApi.DTOs;

namespace TatumBackendApi.Services
{
    public interface IReportingService
    {
        Task<ApiResponse<AdminSummaryDto>> GetAdminSummaryAsync(AdminSummaryRequestDto request, CancellationToken ct = default);
        
    }
}