using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TatumBackendApi.DTOs
{
    public class ProductPurchaseRequestDto
    {
        public Guid AccountId { get; set; }

        public Guid ProductId { get; set; }

        public Guid? ProductItemId { get; set; }

        /// <summary>
        /// Required when product allows custom amount.
        /// </summary>
        public decimal? Amount { get; set; }

        /// <summary>
        /// Dynamic customer fields.
        ///
        /// Example:
        /// {
        ///    "phoneNumber": "08031234567"
        /// }
        /// </summary>
        public Dictionary<string, object?> Fields { get; set; } = new();
    }
}