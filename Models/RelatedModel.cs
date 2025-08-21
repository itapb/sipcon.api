using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Swashbuckle.AspNetCore.Annotations;

namespace Models
{
    public class RelatedModel
    {

        [Required]
        public int PartId { get; set; }

        [Required]
        public int? ModelId { get; set; }

        [Required]
        public bool IsRelated { get; set; }


        [SwaggerIgnore]
        public string? ModelName { get; set; }


        [SwaggerIgnore]
        public string? PartName { get; set; }

        [SwaggerIgnore]
        public int SupplierId { get; set; }

    }

}
