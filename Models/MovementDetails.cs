using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Swashbuckle.AspNetCore.Annotations;


namespace Models
{
    public class MovementDetails
    {


        [Required]
        [Range(1, int.MaxValue)]
        public int? Id { get; set; } = 0;

        [Required]
        [Range(0, int.MaxValue)]
        public int? MovementId { get; set; } = 0;

        [Required]
        [Range(1, int.MaxValue)]
        public int? PartId { get; set; } = 0;

       
        public int? PartialTypeId { get; set; }

        [SwaggerIgnore]
        public string? PartInnerCode { get; set; } = string.Empty;

        [SwaggerIgnore]
        public string? PartBarcode { get; set; } = string.Empty;

        [SwaggerIgnore]

        public string? PartDescription { get; set; } = string.Empty;


        [Required]
        [Range(1, int.MaxValue)]
        public int? LocationId { get; set; } = 0;


        [SwaggerIgnore]
        public int? DestinationId { get; set; } = 0;


        [SwaggerIgnore]
        public string DestinationName { get; set; } = string.Empty;


        [SwaggerIgnore]
        [RegularExpression("^[RAD]$")]
        public string? LocationTypeId { get; set; } = string.Empty;

        [SwaggerIgnore]
        public string LocationName { get; set; } = string.Empty;


        [SwaggerIgnore]
        [RegularExpression("^[PE]$")]
        public string? TypeId { get; set; } = string.Empty;

        [SwaggerIgnore]
        public string TypeName { get; set; } = string.Empty;


        [SwaggerIgnore]
        public int? RequiredQty { get; set; } = 0;

        [Required]
        [Range(0, int.MaxValue)]
        public int? RealQty { get; set; } = 0;


        public string? SerialCode { get; set; } = string.Empty;

        [SwaggerIgnore]
        public bool? Processed { get; set; } = false;


        [SwaggerIgnore]
        public string UpdatedBy { get; set; } = string.Empty;

        [SwaggerIgnore]
        public string UpdatedDate { get; set; } = string.Empty;


        [SwaggerIgnore]
        [Range(0, int.MaxValue)]
        public int? UserId { get; set; } = 0;

        [SwaggerIgnore]
        public string UserName { get; set; } = string.Empty;


        [SwaggerIgnore]
        public int Mapping { get; set; } = 0;


        [SwaggerIgnore]
        public decimal Cost { get; set; } = 0;

        [SwaggerIgnore]
        public decimal SubTotal { get; set; } = 0;

        [SwaggerIgnore]
        public int? Stock { get; set; } = 0;

        [SwaggerIgnore]
        public bool? Serializable { get; set; } = false;

    }
}
