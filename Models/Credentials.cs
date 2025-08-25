using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Credentials: Record
    {

        [Required]
        public String? Login { get; set; }
        
        public String? Password { get; set; }

        public String? prueba { get; set; }


    }

}
