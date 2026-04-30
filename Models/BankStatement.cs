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
        
    }
}
