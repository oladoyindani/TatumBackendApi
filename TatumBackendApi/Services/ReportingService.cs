using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TatumBackendApi.Data;
using TatumBackendApi.DTOs;
using TatumBackendApi.Common.Constants;

namespace TatumBackendApi.Services
{
    public class ReportingService : IReportingService
    {
        private readonly AppDbContext _context;
        public ReportingService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse<AdminSummaryDto>> GetAdminSummaryAsync(
            AdminSummaryRequestDto request, CancellationToken ct = default)
        {
            if (request == null || request.Period is null)
            {
                return ApiResponse<AdminSummaryDto>.Fail(
                    "Period is required. Use yesterday, last 7days, last 30days, or last 90days."
                );
            }

            var today = DateTime.UtcNow.Date;
            var period = request.Period.Value;

            DateTime fromDate;
            DateTime toDate;

            switch (period)
            {
                case Transaction.TransactionPeriod.Yesterday:
                    fromDate = today.AddDays(-1);
                    toDate = today;
                    break;

                case Transaction.TransactionPeriod.Last7Days:
                    fromDate = today.AddDays(-6);
                    toDate = today.AddDays(1);
                    break;

                case Transaction.TransactionPeriod.Last30Days:
                    fromDate = today.AddDays(-29);
                    toDate = today.AddDays(1);
                    break;

                case Transaction.TransactionPeriod.Last90Days:
                    fromDate = today.AddDays(-89);
                    toDate = today.AddDays(1);
                    break;

                default:
                    return ApiResponse<AdminSummaryDto>.Fail(
                        "Invalid period. Use yesterday, last 7days, last 30days, or last 90days."
                    );
            }

            if (fromDate >= toDate)
            {
                return ApiResponse<AdminSummaryDto>.Fail("From Date should be earlier that To Date.");
            }

            var totalUsers = await _context.Users.CountAsync(ct);
            var activeUsers = await _context.Users.CountAsync(user => user.IsActive, ct);
            var totalAccounts = await _context.Accounts.CountAsync(ct);
            var totalBalace = await _context.Accounts.SumAsync(account => account.AvailableBalance, ct);
            var totalTransactions = await _context.Transactions.CountAsync(ct);
            var response = new AdminSummaryDto
            {
                Period = period.ToString(),
                FromDate = fromDate,
                ToDate = toDate.AddTicks(-1),
                TotalUsers = totalUsers,
                ActiveUsers = activeUsers,
                TotalAccounts = totalAccounts,
                TotalBalance = totalBalace,
                TotalTransactions = 0

            };

            return ApiResponse<AdminSummaryDto>.Ok(response, "Admin summary has be retrieved.");
        }

    }
}