using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Swashbuckle.AspNetCore.Annotations;

namespace Models
{
    public class ClaimDetails 
    {
        [Required]
        public int? Id { get; set; }
        [Required]
        public int? ClaimId { get; set; }

        [Required]
        public int? PartId { get; set; }

        [Required]
        public int? Quantity { get; set; } 

        [Required]
        public int? ReplacementPartId { get; set; }

        [Required]
        public int? ReasonId { get; set; }

        [SwaggerIgnore]
        public string? PartInnerCode { get; set; }

        [SwaggerIgnore]
        public string? PartName { get; set; }

        [SwaggerIgnore]
        public string? RemplacemetInnerCode { get; set; }

        [SwaggerIgnore]  
        public string? ReplacemetName { get; set; } 

        [SwaggerIgnore]
        public string? ReasonDescription { get; set; }

        [SwaggerIgnore]
        public string? StatusName { get; set; }
    }

}
 