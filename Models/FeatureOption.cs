using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Swashbuckle.AspNetCore.Annotations;

namespace Models
{
    public class FeatureOption

    {
        [Required]
        public int? Id { get; set; } = 0;


        [Required]
        public bool? IsActive { get; set; } = true;

        [Required]
        public string? Name { get; set; }  

        [Required] 
        public int? FeatureId { get; set; }

        [Required]
        public int? OrderBy { get; set; } = 0;
    }
}
