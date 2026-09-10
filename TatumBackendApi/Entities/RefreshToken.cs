namespace TatumBackendApi.Entities
{
    public class RefreshToken
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        //Refresh token value

        public string Token { get; set; } = string.Empty;

        //UTC expiration date/time.

        public DateTime ExpiresAt { get; set; }

        // Indicates number teh tokien has been revoked
        public bool Revoked { get; set; }

        //UTC creation date/time.

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // User that owns this refresh token.

        public User User { get; set; } = null!;
    }
}
