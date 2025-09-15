using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Swashbuckle.AspNetCore.Annotations;

namespace Models
{
    public class Related
    {
        [Required]
        public int? RecordId { get; set; }

        [SwaggerIgnore]
        public string? RecordName { get; set; }

        [Required]
        public bool IsRelated { get; set; }

        [Required]
        public int RelatedId { get; set; }

        [SwaggerIgnore]
        public string? RelatedName { get; set; }

        [SwaggerIgnore]
        public string? Reference { get; set; }

        [SwaggerIgnore]
        public bool? IsSupplier { get; set; }

        [SwaggerIgnore]
        public bool? IsDealer { get; set; }

        [SwaggerIgnore]
        public int SupplierId { get; set; }

    }
}
