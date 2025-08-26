using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Module
    {
        public int Id { get; set; }
        public string? Name { get; set; }

        public string? ActionName { get; set; }

        public string? ActionDisplay { get; set; }
        
        [SwaggerIgnore]
        public List<ActionModule> Actions { get; set; } = new List<ActionModule>();

    }

}
