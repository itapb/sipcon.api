using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Printqueue
    {
        [SwaggerIgnore]
        public int? Id { get; set; }

        [Required]
        public string? Type { get; set; }
        
        [Required]
        public int? RecordId { get; set; }

        [SwaggerIgnore]
        public DateTime? Printed { get; set; }
        
        [SwaggerIgnore]
        public string? ZPL { get; set; }
    }
}
