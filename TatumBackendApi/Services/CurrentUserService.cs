using System.Security.Claims;

namespace TatumBackendApi.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(
            IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        private ClaimsPrincipal? User =>
            _httpContextAccessor
                .HttpContext?
                .User;

        public Guid UserId
        {
            get
            {
                var value =
                    User?.FindFirst(
                        ClaimTypes.NameIdentifier)?.Value;

                return Guid.TryParse(value, out var id)
                    ? id
                    : Guid.Empty;
            }
        }

        public string? Email =>
            User?.FindFirst(
                ClaimTypes.Email)?.Value;

        public string? Role =>
            User?.FindFirst(
                ClaimTypes.Role)?.Value;

        public bool IsAuthenticated =>
            User?.Identity?.IsAuthenticated
            ?? false;

        public bool IsAdmin =>
            User?.IsInRole("Admin")
            ?? false;

        public bool IsSuperAdmin =>
            User?.IsInRole("SuperAdmin")
            ?? false;
    }
}