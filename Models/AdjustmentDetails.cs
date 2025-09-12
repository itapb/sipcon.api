using System;
using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

namespace Models
{
    public class AdjustmentDetails:Record
    { 
        [Required]
        public int? AdjustmentId { get; set; }

        [Required]
        public int? LocationId { get; set; }

        [Required]
        public int? PartId { get; set; }

        [Required] 
        public int? Stock { get; set; }

        [Required]
        public int? ReasonId { get; set; }

        [Required]
        public string? AdjustmentType { get; set; } 

        [SwaggerIgnore]
        public string? Zone { get; set; }
        
        [SwaggerIgnore]
        public string? Warehouse { get; set; }

        [SwaggerIgnore]
        public string? Location { get; set; }

        [SwaggerIgnore]
        public string? Inncercode { get; set; }

        [SwaggerIgnore]
        public string? PartDescription { get; set; } 

        [SwaggerIgnore]
        public DateTime? DCreated { get; set; }

        [SwaggerIgnore]
        public string? ReasonDescription { get; set; }
    }
}
 