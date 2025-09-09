using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

namespace Models
{
    public class Vehicle: Record
    {

        [Required]
        public string? Vin { get; set; }
       
        [Required]
        public string? EngineSerial { get; set; }
        
        //[Required]
        //public string? BodySerial { get; set; }
        
        [Required]
        public string? Plate { get; set; }
        
        [Required]
        [Range(1, int.MaxValue)]
        public Int32? ColorId { get; set; }

        [SwaggerIgnore]
        public string? ColorName { get; set; }
        
        [Required]
        [Range(1, int.MaxValue)]
        public Int32? ModelId { get; set; }

        [SwaggerIgnore]
        public string? ModelName { get; set; }
        
        [SwaggerIgnore]
        public Int32? BrandId { get; set; }

        [SwaggerIgnore]
        public string? BrandName { get; set; }
        
        [Required]
        [Range(1900, int.MaxValue)]
        public Int32? Year { get; set; }
        
        [Required]
        [Range(1, int.MaxValue)]
        public Int32? SupplierId { get; set; }

        [SwaggerIgnore]
        public string? SupplierName { get; set; }
        
        public Int32? DealerId { get; set; }

        [SwaggerIgnore]
        public string? DealerName { get; set; }
        
        public Int32? CustomerId { get; set; }

        [SwaggerIgnore]
        public string? CustomerName { get; set; }
        [SwaggerIgnore] public String? EstatusId { get; set; }
        [SwaggerIgnore] public String? EstatusName { get; set; }
        [SwaggerIgnore] public String? DealerReference { get; set; }
        [SwaggerIgnore] public String? SupplierReference { get; set; }

        [SwaggerIgnore]
        public Int32? PolicyTypeId { get; set; }

        [SwaggerIgnore]
        public string? PolicyTypeName { get; set; }
        [SwaggerIgnore] public string? RowReference { get; set; }


    }
}
