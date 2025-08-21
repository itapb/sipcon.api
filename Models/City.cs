using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class City: Record
    {
        [Required]
        public string? State { get; set; }

        [Required]
        public string? Name { get; set; }
    }
}
