using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class CredentialLogin
    {
        
        public Int32? UserId { get; set; }

        [Required]
        public String? Login { get; set; }

    }

}
