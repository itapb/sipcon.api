using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Swashbuckle.AspNetCore.Annotations;

namespace Models
{
    public class DetailInspection : Record
    {
        [Required]
        public bool? Value { get; set; }

        [Required]
        public String? Observation { get; set; } = "S/O";

        public String? FileUrl { get; set; }

        [Required]
        public int? IdInspection { get; set; }

        [Required]
        public int? IdFeature { get; set; }

        [SwaggerIgnore]
        public String? NameFeature { get; set; }

        [SwaggerIgnore]
        public String? NameFeatureType { get; set; }
    }
}