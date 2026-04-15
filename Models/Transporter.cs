using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Transporter  
    {
        [SwaggerIgnore]
        public int? Id { get; set; }  
        [SwaggerIgnore]
        public string? FirstName { get; set; } = string.Empty;
    }
}
