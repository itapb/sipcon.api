using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Dealer 
    {
        [SwaggerIgnore]
        public int? Id { get; set; }  
        [SwaggerIgnore]
        public string? Name { get; set; } = string.Empty;

        [SwaggerIgnore]
        public string? Reference { get; set; } = string.Empty;
    }
}
