using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Swashbuckle.AspNetCore.Annotations;

namespace Models
{
    public class Inventory: Record
    {
  

        [Required]
        [Range(1, int.MaxValue)]
        public Int32? LocationId { get; set; }
        
        [SwaggerIgnore]
        public string? LocationName { get; set; }


        [SwaggerIgnore]
        public Int32? ZoneId { get; set; }

        [SwaggerIgnore]
        public string? ZoneName { get; set; }

        [SwaggerIgnore]
        public string? ZoneSize { get; set; }


        [SwaggerIgnore]
        public Int32? WarehouseId { get; set; }

        [SwaggerIgnore]
        public string? WarehouseName { get; set; }

        [SwaggerIgnore]
        public Int32? SupplierId { get; set; }


        [SwaggerIgnore]
        public string? SupplierName { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public Int32? PartId { get; set; }

        [SwaggerIgnore]
        public string? PartSize { get; set; }

        [SwaggerIgnore]
        public string? PartInnerCode { get; set; }

        [SwaggerIgnore]
        public string? PartName { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public Int32? Stock { get; set; }

        [SwaggerIgnore]
        public decimal? Price { get; set; } = decimal.Zero;
         
    }
}
