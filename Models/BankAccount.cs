using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Models
{
    public class BankAccount
    {
        [Required] public String? Code { get; set; }
        [Required] public String? Name { get; set; }
        [Required] public String? Account { get; set; }
    }
}
