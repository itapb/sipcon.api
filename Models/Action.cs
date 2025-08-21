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
    public class Action
    {

        [SwaggerIgnore]
        public int? UserId { get; set; }

        [Required]
        public int? RecordId { get; set; }

        [Required]
        public int? ModuleId { get; set; }

        [Required]
        public string? ActionName { get; set; }

        public string? ActionComment { get; set; }

        public Int32 RelatedId { get; set; }


    }
}
