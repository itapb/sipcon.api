using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Swashbuckle.AspNetCore.Annotations;

namespace Models
{

    public class SaleOrderDetail : Record
    {
        [Required]
        public int? Id { get; set; }
        
        [Required]
        public int? SaleOrderId { get; set; }

        [Required]
        public int? PartId { get; set; }

        [SwaggerIgnore]
        public string? PartInnerCode { get; set; }

        [SwaggerIgnore]
        public string? PartName { get; set; }

        [Required]
        public int? Quantity { get; set; }

        [SwaggerIgnore]
        public int? Invoiced { get; set; }

        public int? ReasonId { get; set; }

        [SwaggerIgnore]
        public decimal? Cost { get; set; }

        [SwaggerIgnore]
        public decimal? Price { get; set; }

        [SwaggerIgnore]
        public int Claim { get; set; }

        [SwaggerIgnore]
        public decimal? TaxAmount { get; set; }

        [SwaggerIgnore]
        public decimal? SubTotal { get; set; }
    }
}
