using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Swashbuckle.AspNetCore.Annotations;

namespace Models
{
    public class Fase:Record
    {
        [Required] public String? Name { get; set; }
        [Required] public int? OrderBy { get; set; }
        [Required] public int? IdArea { get; set; }
        [SwaggerIgnore] public String? NameArea { get; set; }
  }
}
