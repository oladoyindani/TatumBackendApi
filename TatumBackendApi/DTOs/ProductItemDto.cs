using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TatumBackendApi.DTOs
{
    public class ProductItemDto
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal UnitPrice { get; set; }
        // public int Quantity { get; set; }
        // public bool IsActive { get; set; }
    }
}