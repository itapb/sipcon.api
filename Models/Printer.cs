using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Printer
    {
        public int? Id { get; set; }

        [Required]
        public string? Name { get; set; }

        public int? IdSupplier { get; set; }

        public bool? BDefault { get; set; }
    }
}