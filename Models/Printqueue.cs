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

        [SwaggerIgnore]
        public string PrintedDate { get; set; } = "";

        [SwaggerIgnore]
        public string Code { get; set; } = "";

        [SwaggerIgnore]
        public string Description { get; set; } = "";

        [SwaggerIgnore]
        public string? Supplier { get; set; }

        [SwaggerIgnore]
        public string CodePair { get; set; } = "";

        [SwaggerIgnore]
        public string DescriptionPair { get; set; } = "";

        [SwaggerIgnore]
        public int? IdPair { get; set; }

        [SwaggerIgnore]
        public string Note { get; set; } = "";

        [SwaggerIgnore]
        public string NotePair { get; set; } = "";

    
        public int? Quantity { get; set; } = 1;
    }
}
