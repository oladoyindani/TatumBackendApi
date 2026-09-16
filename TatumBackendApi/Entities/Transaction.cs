using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TatumBackendApi.Entities
{
    public class Transaction
    {
      public Guid Id { get; set; }

      public Guid AccountId { get; set; }

      public Account Account { get; set; } = null!;
      public Guid CustomerId { get; set; }
      /// <summary>
      /// Transfer, Airtime, Data, Electricity, CableTV, etc.
      /// </summary>
      public string Type { get; set; } = null!;

      public string Status { get; set; } = "Pending";

      public string Reference { get; set; } = null!;

      public decimal Amount { get; set; }

      public string Currency { get; set; } = "NGN";

      public string? Narration { get; set; }

      /// <summary>
      /// Biller used for the transaction.
      /// Null for account transfers.
      /// </summary>
      public Guid? BillerId { get; set; }

      public Biller? Biller { get; set; }

      /// <summary>
      /// Product used for the transaction.
      /// </summary>
      public Guid? ProductId { get; set; }

      public Product? Product { get; set; }

      /// <summary>
      /// Product item/bundle used.
      /// </summary>
      public Guid? ProductItemId { get; set; }

      public ProductItem? ProductItem { get; set; }

      /// <summary>
      /// Values submitted by the customer.
      ///
      /// Example:
      /// {
      ///  "phoneNumber": "08031234567"
      /// }
      /// </summary>
      public string? CustomerFields { get; set; }

      /// <summary>
      /// Provider's transaction/reference ID.
      /// </summary>
      public string? ProviderReference { get; set; }

      public DateTime CreatedAt { get; set; }
          = DateTime.UtcNow;

      public DateTime? CompletedAt { get; set; }
      public User? User { get; set; }
  }

}