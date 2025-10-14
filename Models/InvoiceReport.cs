using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class InvoiceReport
    {

        [Required]
        public string? Vat { get; set; }

        [Required]
        public string? PartCode { get; set; }

        [Required]
        public int? Quantity { get; set; }

        [Required]
        public string? Reference { get; set; }

        [Required]
        public string? InvoiceNumber { get; set; }



    }
}
