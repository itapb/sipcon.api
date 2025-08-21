using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Swashbuckle.AspNetCore.Annotations;

namespace Models
{
    public class AccessGroupUser  

    {
        [Required] public Int32? UserId { get; set; }
        [Required] public Int32? AccessGroupId { get; set; }
        [Required] public Boolean? Assign { get; set; }
        [SwaggerIgnore] public String? AccessGroup { get; set; }
        [SwaggerIgnore] public String? UserName { get; set; }
        [SwaggerIgnore] public String? UserLastName { get; set; }
        [SwaggerIgnore] public String? Login { get; set; }
    }
}
