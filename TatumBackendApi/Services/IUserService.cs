using TatumBackendApi.Common.Models;
using TatumBackendApi.DTOs;

namespace TatumBackendApi.Services
{
    public interface IUserService
    {
        Task<ApiResponse<PagedResult<UserDto>>> GetUsersAsync(
            GetUsersQueryDto query,
            Guid currentUserId,
            bool isAdmin,
            CancellationToken ct = default);
        Task<ApiResponse<UserDto>> UpdateProfileAsync(
            Guid userId,
            UpdateProfileRequestDto request,
            CancellationToken ct = default);
    }
}
