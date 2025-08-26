
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Swashbuckle.AspNetCore.Annotations;

namespace Models
{
    // test models
    public class ActionModule
    {

        [Required]
        public int? ModuleId { get; set; }
       
        [Required]
        public int? ActionId { get; set; }

        [Required]
        public string? ActionName { get; set; }


    }
}
