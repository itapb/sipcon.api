using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class User : Record
    {
        [SwaggerIgnore]
        public string? Login { get; set; }

        [SwaggerIgnore]
        public string? Name { get; set; }
        
        [SwaggerIgnore]
        public string? LastName { get; set; }
        
        [SwaggerIgnore]
        public string? Vat { get; set; }

   

    }
}
