using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class PartType :Record
    {
        [Required]
        public String? Name { get; set; }
    }
}
