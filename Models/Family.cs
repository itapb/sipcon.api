using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Family : Record
    {

        [Required]
        public String? Name { get; set; }

        [Required]
        public Int32? PartTypeId { get; set; }

        [Required]
        public String? PartTypeName { get; set; }

   
    }
}
