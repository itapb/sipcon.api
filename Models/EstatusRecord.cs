
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
    public class EstatusRecord : Record
    {

       
        [Required]
        public int? EstatusId { get; set; }

        [Required]
        public String? Estatus { get; set; }

        public DateTime Date { get; set; }

        public String? Name { get; set; }

        public String? Display { get; set; }


    }
}
