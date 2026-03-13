using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Swashbuckle.AspNetCore.Annotations;

namespace Models
{
    public class NewMovementDetail
    {
        [Required]
        [Range(0, int.MaxValue)]
        public int? Id { get; set; } = 0;

        [Required]
        [Range(1, int.MaxValue)]
        public int? MovementId { get; set; } = 0;

        [Required]
        [Range(1, int.MaxValue)]
        public int? PartId { get; set; } = 0;

        [Required]
        [Range(1, int.MaxValue)]
        public int? LocationId { get; set; } = 0;

        [Range(0, int.MaxValue)]
        public int? DestinationId { get; set; } = 0;

        [Required]
        [Range(0, int.MaxValue)]
        public int? RequiredQty { get; set; } = 0;

        //public string? SerialCode { get; set; } = string.Empty;

        [SwaggerIgnore]
        public string? InnerCode { get; set; } = string.Empty;

        [SwaggerIgnore]
        public string? LocationName { get; set; } = string.Empty;

        [SwaggerIgnore]
        public string? DestinationName { get; set; } = string.Empty;

        [SwaggerIgnore]
        public int? Stock { get; set; } = 0;

        [SwaggerIgnore]
        public String? RowReference { get; set; }

        [SwaggerIgnore]
        public String? PartName { get; set; }

        [SwaggerIgnore]
        public decimal? Cost { get; set; }

        [SwaggerIgnore]
        public int? CrossDockingQty { get; set; }

        [SwaggerIgnore]
        public int? CrossDockingLocationId { get; set; }

        [SwaggerIgnore]
        public string? CrossDockingName { get; set; }

    }
}
