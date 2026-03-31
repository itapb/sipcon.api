using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Models
{
    public class AccountReceivable : Record
    {
        [Required] public int? SupplierId { get; set; }
        [Required] public int? DealerId { get; set; }
        [Required] public string? Type { get; set; }
        [Required] public string? Concept { get; set; }
        [Required] public string? Number { get; set; }
        [Required] public string? Reference { get; set; }
        [Required] public string? Date { get; set; }
        [Required] public string? DueDate { get; set; }
        [Required] public decimal? Amount { get; set; }
        [Required] public decimal? Balance { get; set; }
        [Required] public decimal? Rate { get; set; }
        [Required] public string? Status { get; set; }


    }
}
