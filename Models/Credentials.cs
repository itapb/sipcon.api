using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Credentials :CredentialLogin
    {

        [Required] public String? Password { get; set; }
        [Required] public String? TemporaryKey { get; set; }
        public String? Salt { get; set; }

    }

}
