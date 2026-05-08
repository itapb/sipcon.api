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
        [Required] public decimal? Balance { get; set; }
        [Required] public DateTime? Date { get; set; }
        public List<PaymentDetails> PaymentDetails { get; set; } = new List<PaymentDetails>();

    }
    public class PaymentDetails 
    {
        public int? Id { get; set; }
        public int? PaymentId{ get; set; }
        [Required] public DateTime? Date { get; set; }
        [Required] public decimal? Amount { get; set; }
        [Required] public decimal? AmountBs { get; set; }
        [Required] public decimal? Rate { get; set; }
        [Required] public DateTime? DateRate { get; set; }
        [SwaggerIgnore] public string? CurrencyName { get; set; }
        [Required] public int? CurrencyId { get; set; }
        [SwaggerIgnore] public string? TypeName { get; set; }
        [Required] public int? TypeId { get; set; }
        [Required] public string? Reference { get; set; }
         public int? BankId { get; set; }
        [SwaggerIgnore] public string? BankName { get; set; }
         public int? AccountId { get; set; }
        [SwaggerIgnore] public string? AccountNumber { get; set; }
        [SwaggerIgnore] public string? BankOriginName { get; set; }
        [SwaggerIgnore] public int? BankOriginId { get; set; }
        [Required] public int? DealerId { get; set; }
        [Required] public int? SupplierId { get; set; }
        [SwaggerIgnore] public string? DealerName { get; set; }
        [SwaggerIgnore] public string? StatusName { get; set; }
        [SwaggerIgnore] public int? StatusId { get; set; }

    }

    public class PostPaymentDetail 
    {
        public int? Id { get; set; }
        public int? PaymentId { get; set; }
        [Required] public DateTime? Date { get; set; }
        [Required] public decimal? Amount { get; set; }
        [Required] public int? CurrencyId { get; set; }
        [Required] public int? TypeId { get; set; }
        [Required] public string? Reference { get; set; }
        public int? AccountId { get; set; }
        [Required] public int? DealerId { get; set; }
        [Required] public int? SupplierId { get; set; }
        public int? BankOriginId { get; set; }
        public List<Settlements>? Settlements { get; set; } = new List<Settlements>();
    }

    public class Settlements 
    {
         public int? DocumentId { get; set; }
         public decimal? Rate { get; set; }
         public DateTime? DateRate { get; set; }

    }


    public class PaymentStatus
    {
        [SwaggerIgnore] public int? Id { get; set; }
        [SwaggerIgnore] public string? Name { get; set; }
        [SwaggerIgnore] public int? Count { get; set; }

    }


    public class PaymentFull:PaymentDetails
    {
        
        public List<AccountPreview>? AccountPreview { get; set; } = new List<AccountPreview>();

    }

}
