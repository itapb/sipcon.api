using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class City: Record
    {
  
        [Required]
        public string? Name { get; set; }

        [Required]
        public int? MunicipalityId { get; set; }

        [SwaggerIgnore]
        public string? MunicipalityName { get; set; }

        [Required]
        public int? StateId { get; set; }

        [SwaggerIgnore]
        public string? StateName { get; set; }
    }
}
