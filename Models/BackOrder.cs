using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Swashbuckle.AspNetCore.Annotations;

namespace Models
{
    public class BackOrder : Record
    {

        [SwaggerIgnore]
        public string? SupplierId { get; set; }
       
        [Required]
        public int? Quantity { get; set; }

     
        public DateTime? Arrival { get; set; }

        [SwaggerIgnore]
        public string? PartDescription { get; set; }

        [SwaggerIgnore]
        public string? PartInnerCode { get; set; }

        [SwaggerIgnore]
        public int? StatusId { get;set; }
        [SwaggerIgnore]
        public DateTime? CreatedDate { get; set; }
        [SwaggerIgnore]
        public int? DealerId { get; set; }
        [SwaggerIgnore]
        public string? DealerName { get; set; }
        [SwaggerIgnore]
        public int? SaleOrderNumber { get; set;}
        [SwaggerIgnore]
        public int? TypeId{ get; set; }
        [SwaggerIgnore]
        public string? TypeName { get; set; } 

        [SwaggerIgnore]
        public string? SupplierRef { get; set; }
        [SwaggerIgnore]
        public string? DealerRef { get; set; }


    }

}
