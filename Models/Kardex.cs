using System;
using System.Collections.Generic;
using System.Text;

namespace Models
{
    public class Kardex: Record
    {
        public Int32? SupplierId { get; set; }
        public Int32? PartId { get; set; }
        public string? PartCode { get; set; }
        public string? PartName { get; set; }
        public Int32? ReferenceId { get; set; }
        public string? Type { get; set; }
        public Int32? Quantity { get; set; }
        public Int32? QuantityOld { get; set; }
        public Int32? QuantityNew { get; set; }
        public decimal? Cost { get; set; }
        public string? Created { get; set; }
        public string? UserName { get; set; }

    }
}
