using System;
using System.Collections.Generic;
using System.Text;

namespace Models
{
    public class BackOrderTemplate
    {
     
        public string? Vat { get; set; } = string.Empty;
        public string? DealerName { get; set; } = string.Empty;
        public string? SaleOrderType { get; set; } = string.Empty;
        public string? InnerCode { get; set; } = string.Empty;
        public string? Description { get; set; } = string.Empty;
        public int? Quantity { get; set; }

    }
}
