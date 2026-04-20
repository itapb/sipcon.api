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
        [Required] public string? DealerName { get; set; }
        [Required] public string? TypeCode { get; set; }
        [Required] public string? TypeName { get; set; }
        [Required] public string? ConceptCode { get; set; }
        [Required] public string? ConceptName { get; set; }
        [Required] public string? Number { get; set; }
        [Required] public string? Reference { get; set; }
        [Required] public string? DocumentDate { get; set; }
        [Required] public string? DocumentDueDate { get; set; }
        [Required] public decimal? Amount { get; set; }
        [Required] public decimal? AmountBs { get; set; }
        [Required] public decimal? Balance { get; set; }
        [Required] public decimal? BalanceBs { get; set; }
        [Required] public decimal? Rate { get; set; }
        [Required] public string? StatusName { get; set; }
        [Required] public int? StatusId { get; set; }
    }


    public class AccountPreview 
    {
        [Required] public int? Id { get; set; }
        [Required] public string? Number { get; set; }
        [Required] public string? DocumentDate { get; set; }
        [Required] public decimal? Amount { get; set; }

    }


}
