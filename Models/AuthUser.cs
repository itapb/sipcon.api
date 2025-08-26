using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class AuthUser
    {
        
      
        [Required] public String? Login { get; set; }
        [Required] public String? Password { get; set; }


    }

}
