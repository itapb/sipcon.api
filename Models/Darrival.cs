using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Swashbuckle.AspNetCore.Annotations;

namespace Models
{
    public class Darrival
    { 
        public int? Id { get; set; }
        public DateTime? Arrival { get; set; }

    }

}
