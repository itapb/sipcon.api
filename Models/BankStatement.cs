using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Models
{
    public class BankStatement
    {
        [Required] public String? BankAccount { get; set; }
        [Required] public String? BankCode { get; set; }
        [Required] public DateTime? TransactionDate { get; set; }
        [Required] public String? Reference { get; set; }
        [Required] public decimal? Amount { get; set; }
        [SwaggerIgnore] public int? PaymentDetailId { get; set; }
        [SwaggerIgnore] public int? Id { get; set; }
        [SwaggerIgnore] public String? Estatus { get; set; }
        [Required] public DateTime? Created { get; set; }

    }
}
