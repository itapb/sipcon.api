using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Swashbuckle.AspNetCore.Annotations;

namespace Models
{
    public class Feature : Record
    {
        [Required]
        public String? Name { get; set; }

        [Required]
        public int? IdFeatureType { get; set; }

        [SwaggerIgnore]
        public String? NameFeatureType { get; set; }

        [Required]
        public int? IdModel { get; set; }

        [SwaggerIgnore]
        public String? NameModel { get; set; }
    }
}