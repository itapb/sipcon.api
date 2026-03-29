using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Swashbuckle.AspNetCore.Annotations;

namespace Models
{
    public class FeatureType : Record
    {
        [Required]
        public String? Name { get; set; }

        [Required]
        public int? IdFase { get; set; }

        [SwaggerIgnore]
        public String? NameFase { get; set; }
    }
}