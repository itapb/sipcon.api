using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class RefreshRequest

    {
        public string Username { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;

    }
}
