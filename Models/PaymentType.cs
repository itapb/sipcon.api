using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Models
{
    public class PaymentType : Record
    {
        [Required] public String? Name { get; set; }
    }
}
