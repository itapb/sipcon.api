using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Swashbuckle.AspNetCore.Annotations;


namespace Models
{
    public class Item : Record
    {

       [Required] public String? Description { get; set; }
       [Required] public String? Type { get; set; }
       [Required] public Int32? SupplierId { get; set; }

    }
}
