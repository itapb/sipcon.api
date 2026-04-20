using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Swashbuckle.AspNetCore.Annotations;

namespace Models
{
    public class Feature : Record

    {
        [Required]
        public string? Name { get; set; } 
        public bool? DefaultValue { get; set; }

        [Required]
        public int? FeatureTypeId { get; set; }

        [Required]
        public int? FeatureValueTypeId { get; set; }

        [Required]
        public int? ModelId { get; set; }

        [Required]
        public int? SupplierId { get; set;} 
        public int? DealerId { get; set; }

        [SwaggerIgnore]
        public string? FeatureTypeName { get; set; }

        [SwaggerIgnore]
        public string? ModelName { get; set; }

        [SwaggerIgnore]
        public int? FaseId { get; set; }

        [SwaggerIgnore]
        public string? FaseName { get; set; }

        [SwaggerIgnore]
        public int? AreaId { get; set; }

        [SwaggerIgnore]
        public string? AreaName { get; set; } 

        [SwaggerIgnore]
        public string? SupplierName { get; set; } 

        [SwaggerIgnore]
        public string? DealerName { get; set; }

        [SwaggerIgnore]
        public DateTime? Updated { get; set; }

        [SwaggerIgnore]
        public string? UpdatedBy { get; set; }
    }
}
