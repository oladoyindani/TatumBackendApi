using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TatumBackendApi.DTOs
{
    public class ProductDto
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal? UnitPrice { get; set; }
        public bool AllowsCustomAmount { get; set; }
        public bool RequiresProductItem { get; set; }
        public string? RequiredFields { get; set; }
        public List<ProductItemDto> ProductItems { get; set; } = new();
    }
}