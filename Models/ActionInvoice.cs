using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Swashbuckle.AspNetCore.Annotations;

namespace Models
{
    // test models
    public class ActionInvoice: Action
    {

        
        [Required]
        public String? InvoiceNumber { get; set; }

        [Required]
        public DateTime? InvoiceDate { get; set; }

        

    }
}
