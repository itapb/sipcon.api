using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Swashbuckle.AspNetCore.Annotations;

namespace Models
{
    public class Attachment : Record
    {
        [Required] public String? FileName { get; set; }
        [Required] public Int32? RecordId { get; set; }
        [SwaggerIgnore] public Int32? ModuleId { get; set; }
        [Required] public String? ModuleName { get; set; }


    }
}