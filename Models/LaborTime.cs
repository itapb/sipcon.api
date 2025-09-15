using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class LaborTime : Record
    {
      
        [Required]
        public Int32? ModelId { get; set; }
        [Required]
        public String? ModelName { get; set; }
        [Required]
        public string? Reference { get; set; }

        [Required]
        public string? Description { get; set; }

        [Required]
        public Decimal? Hours { get; set; }
        [SwaggerIgnore]
        public Decimal? Price { get; set; }
        [SwaggerIgnore]
        public String? RowReference { get; set; }

    }
}
