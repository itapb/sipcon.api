
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
    public class EstatusRecord
    {

       
        [Required]
        public int? EstatusId { get; set; }

        [Required]
        public String? Estatus { get; set; }

        public DateTime Date { get; set; }

        [Required]
        public String? DealerSaleName { get; set; }

        [Required]
        public String? DealerSaleCod { get; set; }




    }
}
