using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Companies : Record
    {

        [SwaggerIgnore]
        public string? Name { get; set; }

        [SwaggerIgnore]
        public string? Type { get; set; }
        
        [SwaggerIgnore]
        public string? SupplierId { get; set; }

    }
}
