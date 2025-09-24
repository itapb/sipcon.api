using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

namespace Models
{
    public class SaleOrder: Record
    {   
        [Required]
        public int? Id { get; set; }

        [Required]
        public int? DealerId { get; set; }

        [SwaggerIgnore]
        public string? DealerName { get; set; }

        [Required]
        public int? SupplierId { get; set; }

        [SwaggerIgnore]
        public string? SupplierName { get; set; }


        [SwaggerIgnore]
        public int? StatusId { get; set; }

        [SwaggerIgnore]
        public string? StatusName { get; set; }

        [Required]
        public int? TypeId { get; set; }

        [SwaggerIgnore]
        public string? TypeName { get; set; }

        public string? Reference { get; set; }

      
        public string? Comment { get; set; }


        [SwaggerIgnore]
        public string Created { get; set; } = "";


        [SwaggerIgnore]
        public string CreatedBy { get; set; } = string.Empty;

        [SwaggerIgnore]
        public bool IsClaim { get; set; } = false;

        [Required]
        public bool Paralyzed { get; set; } 

        [Required]
        public int VehicleId { get; set; } 

        [SwaggerIgnore]
        public string? VehicleVin { get; set; } 



    }
}
