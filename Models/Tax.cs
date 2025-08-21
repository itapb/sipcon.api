using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Tax: Record
    {


        [Required]
        public String? Name { get; set; }

        [Required]
        public decimal? Amount { get; set; }

    }
}
