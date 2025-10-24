using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Swashbuckle.AspNetCore.Annotations;

namespace Models
{
    public class GuideDetails
    {

        [Required]
        public int? Id { get; set; }

        [Required]
        public int? ReasonId { get; set; }

        [SwaggerIgnore]
        public string? ReasonDescription { get; set; }

        [SwaggerIgnore]
        public int? GuideId { get; set; }

        [SwaggerIgnore]
        public int? PartId { get; set; }

        [SwaggerIgnore]
        public string? PartInnercode { get; set; }

        [SwaggerIgnore]
        public string? PartDescription { get; set; }

        [SwaggerIgnore]
        public int Quantity { get; set; }

        [SwaggerIgnore]
        public string? PackageId { get; set; }

        [SwaggerIgnore]
        public int? PackageNumber { get; set; }

        [SwaggerIgnore]
        public string? PackageCode { get; set; }
        
        
       
        public int? Received {  get; set; }
    
        public string? Observation { get; set; }


        [SwaggerIgnore]

        public int? SaleOrderId { get; set; }

     
        public bool? Confirmed { get; set; }

    }
}
