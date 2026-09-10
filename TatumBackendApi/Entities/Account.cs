using System.Diagnostics.Eventing.Reader;

namespace TatumBackendApi.Entities
{
    public class Account
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public string AccountNumber { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Currency {  get; set; } = "NGN";
        public decimal LedgerBalance { get; set; }
        public decimal AvailableBalance { get; set; }
        public string Status { get; set; } = "InActive";
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; }

        // Navigation property
        public User Customer { get; set; } = null!;
    }
}
