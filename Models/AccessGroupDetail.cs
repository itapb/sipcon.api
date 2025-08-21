using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Swashbuckle.AspNetCore.Annotations;

namespace Models
{
    public class AccessGroupDetail 

    {
        [Required] public Int32? AccessGroupId { get; set; }
        [Required] public Int32? ModuleId { get; set; }
        [Required] public Int32? ActionId { get; set; }
        [Required] public Boolean ? Assign { get; set; }
        [SwaggerIgnore] public String? Action { get; set; }
        [SwaggerIgnore] public String? Module { get; set; }
        [SwaggerIgnore] public String? ActionDisplay { get; set; }

    }
}
