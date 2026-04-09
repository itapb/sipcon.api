using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Models
{


    public class Payment : Record
    {
        [Required] public int? DealerId { get; set; }
        [Required] public int? SupplierId { get; set; }
        [Required] public decimal? Amount { get; set; }
        [Required] public DateTime? Date { get; set; }
        public List<PaymentDetails> PaymentDetails { get; set; } = new List<PaymentDetails>();

    }
    public class PaymentDetails : Record
    {
        
        [Required] public DateTime? Date { get; set; }
        [Required] public decimal? Amount { get; set; }
        [SwaggerIgnore] public decimal? Rate { get; set; }
        [SwaggerIgnore] public DateTime? DateRate { get; set; }
        [SwaggerIgnore] public string? CurrencyName { get; set; }
        [Required] public int? CurrencyId { get; set; }
        [SwaggerIgnore] public string? TypeName { get; set; }
        [Required] public int? TypeId { get; set; }
        [Required] public string? Reference { get; set; }
        [SwaggerIgnore] public int? BankId { get; set; }
        [SwaggerIgnore] public string? BankName { get; set; }
         public int? AccountId { get; set; }
        [SwaggerIgnore] public string? AccountNumber { get; set; }
        [Required] public int? DealerId { get; set; }
        [Required] public int? SupplierId { get; set; }
        [SwaggerIgnore] public string? DealerName { get; set; }
        [SwaggerIgnore] public string? StatusName { get; set; }
        [SwaggerIgnore] public int? StatusId { get; set; }


    }
}
