using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Swashbuckle.AspNetCore.Annotations;

namespace Models
{
    public class Location : Record
    {


        [Required]
        public string? Name { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public Int32? ZoneId { get; set; }

        [SwaggerIgnore]
        public string? ZoneName { get; set; }

        [SwaggerIgnore]
        public string? WarehouseName { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public Int32? Mapping { get; set; }

        [Required]
        [RegularExpression("^[RAD]$")]
        public string? TypeId { get; set; } = string.Empty;

        [SwaggerIgnore]
        public string? TypeName { get; set; } = string.Empty;


    }
}
