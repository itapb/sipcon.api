using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Swashbuckle.AspNetCore.Annotations;

namespace Models
{
    public class Comment : Record
    {


        [Required] public String? Content { get; set; }
        [Required] public Int32? RecordId { get; set; }
        [SwaggerIgnore] public Int32? ModuleId { get; set; }
        [Required] public String? ModuleName { get; set; }
        [SwaggerIgnore] public String? UserName { get; set; }
        [SwaggerIgnore] public String? UserLastName { get; set; }
        [SwaggerIgnore] public String? UserType { get; set; }
        [SwaggerIgnore] public DateTime? DateComment { get; set; }



    }
}