using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TatumBackendApi.DTOs;

namespace TatumBackendApi.Services
{
    public interface INotificationService
    {
        // Task<ApiResponse<NotificationDto>> SendAsync(
        //     Guid customerId,
        //     string subject,
        //     string message,
        //     string type = "General",
        //     CancellationToken ct = default
        // );

        // Task<ApiResponse<object>> SendAsync(
        //     Guid customerId,
        //     string subject,
        //     string message
        // );

        Task<ApiResponse<object>> SendEmailAsync(
            string email,
            string subject,
            string htmlMessage,
            CancellationToken ct = default
        );

        Task<ApiResponse<object>> SendSmsAsync(
            string phone,
            string message,
            CancellationToken ct = default
        );

    //     Task<ApiResponse<List<NotificationDto>>>
    //         GetByCustomerIdAsync(
    //             Guid customerId,
    //             CancellationToken ct = default
    //         );
    }
}