using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class InventoryTamplate
    {

        public string? InnerCode { get; set; } = string.Empty;
        public string? MasterCode { get; set; } = string.Empty;
        public string? Description { get; set; } = string.Empty;
        public decimal? Price { get; set; } = 0;
        public decimal? Cost { get; set; } = 0;
        public string? Tax { get; set; } = string.Empty;
        public string? Type { get; set; } = string.Empty;
        public string? Family { get; set; } = string.Empty;
        public string? SubFamily { get; set; } = string.Empty;
        public string? Size { get; set; } = string.Empty;

        public string? Warehouse { get; set; } = string.Empty;

        public string? Zone { get; set; } = string.Empty;

        public string? Location { get; set; } = string.Empty;

        public int? Stock { get; set; } = 0;

    }
}
