using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Swashbuckle.AspNetCore.Annotations;

namespace Models
{
    public class FeatureValueType

    {
        [SwaggerIgnore]
        public string? Id { get; set; }

        [SwaggerIgnore]
        public string? Name { get; set; } 
        
    }
}
