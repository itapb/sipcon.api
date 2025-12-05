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

        public string PrintedDate { get; set; } = "";

        public string Code { get; set; } = "";

        public string Description { get; set; } = "";

        public string? Supplier { get; set; }


        public string CodePair { get; set; } = "";

        public string DescriptionPair { get; set; } = "";

        public int? IdPair { get; set; }

        public string Note { get; set; } = "";

        public string NotePair { get; set; } = "";
    }
}
