using TatumBackendApi.Common.Models;
using TatumBackendApi.DTOs;
using TatumBackendApi.Entities;
using TatumBackendApi.Repositories;
using TatumBackendApi.Responses;

namespace TatumBackendApi.Services
{
    public class UserServices : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IAccountRepository _accountRepository;

        public UserServices(IUserRepository userRepository, IAccountRepository accountRepository)
        {
            _userRepository = userRepository;
            _accountRepository = accountRepository;
        }

        public async Task<ApiResponse<PagedResult<UserDto>>> GetUsersAsync(
            GetUsersQueryDto query,
            Guid currentUserId,
            bool isAdmin,
            CancellationToken ct = default)
        {
            // Non-admins can never see anyone but themselves —2
            // override whatever UserId they passed, don't just reject it
            var effectiveUserId = isAdmin ? query.UserId : currentUserId;

            var pagination = new PaginationParameters
            {
                PageNumber = query.PageNumber < 1 ? 1 : query.PageNumber,
                PageSize = query.PageSize < 1 ? 10 : query.PageSize
            };

            var result = await _userRepository.GetPagedAsync(effectiveUserId, pagination, ct);

            // Fetch accounts for every user on this page in one query
            var userIds = result.Items.Select(u => u.Id).ToList();
            var accounts = await _accountRepository.GetByCustomerIdsAsync(userIds, ct);
            var accountsByUser = accounts
                .GroupBy(a => a.CustomerId)
                .ToDictionary(g => g.Key, g => g.ToList());

            var dtoResult = new PagedResult<UserDto>
            {
                Items = result.Items.Select(MapToDto).ToList(),
                PageNumber = result.PageNumber,
                PageSize = result.PageSize,
                TotalCount = result.TotalCount,
                TotalPages = result.TotalPages
            };

            return ApiResponse<PagedResult<UserDto>>.Ok(dtoResult, "Users retrieved successfully.");
        }

        public async Task<ApiResponse<UserDto>> UpdateProfileAsync(
            Guid userId,
            UpdateProfileRequestDto request,
            CancellationToken ct = default)
        {
            var user = await _userRepository.GetByIdAsync(userId);

            if (user == null)
            {
                return ApiResponse<UserDto>.Fail(
                    "User not found.",
                    new List<ApiError> { new("UserNotFound", "User not found.") });
            }

            if (!string.IsNullOrWhiteSpace(request.FirstName))
                user.FirstName = request.FirstName.Trim();

            if (!string.IsNullOrWhiteSpace(request.LastName))
                user.LastName = request.LastName.Trim();

            if (!string.IsNullOrWhiteSpace(request.Phone))
                user.Phone = request.Phone.Trim();

            if (!string.IsNullOrWhiteSpace(request.Department))
                user.Department = request.Department.Trim();

            if (!string.IsNullOrWhiteSpace(request.ProfileImageUrl))
                user.ProfileImageUrl = request.ProfileImageUrl.Trim();

            user.UpdatedAt = DateTime.UtcNow;

            await _userRepository.SaveChangesAsync(ct);

            return ApiResponse<UserDto>.Ok(MapToDto(user), "Profile updated successfully.");
        }

        private static UserDto MapToDto(User user)
        {
            return MapToDto(user, new Dictionary<Guid, List<Account>>());
        }

        private static UserDto MapToDto(User user, Dictionary<Guid, List<Account>> accountsByUser)
        {
            accountsByUser.TryGetValue(user.Id, out var userAccounts);

            return new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Phone = user.Phone,
                Department = user.Department,
                ProfileImageUrl = user.ProfileImageUrl,
                StaffId = user.StaffId,
                Role = user.Role,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt,
                LastLoginAt = user.LastLoginAt,
                Accounts = (userAccounts ?? new List<Account>()).Select(a => new AccountDto
                {
                    Id = a.Id,
                    CustomerId = a.CustomerId,
                    AccountNumber = a.AccountNumber,
                    Name = a.Name,
                    Currency = a.Currency,
                    AvailableBalance = a.AvailableBalance,
                    LedgerBalance = a.LedgerBalance,
                    Status = a.Status
                }).ToList()
            };
        }
    }
}
