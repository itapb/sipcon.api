using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Guide
    {


        [Required]
        public Int32? Id { get; set; }

        [Required]
        public Int32? SupplierId { get; set; }

        [SwaggerIgnore]
        public string? SupplierName { get; set; }

        [Required]
        public Int32? CustomerId { get; set; }

        [SwaggerIgnore]
        public string? CustomerName { get; set; }


        public Int32? ProviderId { get; set; }

        [SwaggerIgnore]
        public string? ProviderName { get; set; }

        public string? Number { get; set; }


        [SwaggerIgnore]
        public DateTime? CreatedDate { get; set; }

        [SwaggerIgnore]
        public DateTime? DeliveredDate { get; set; }

        [SwaggerIgnore]
        public string? StatusName { get; set; }


        [SwaggerIgnore]
        public Int32? UserId { get; set; }


        [SwaggerIgnore]
        public bool? Delivered { get; set; } = false;


        [SwaggerIgnore]

        public bool? Closed { get; set; } = false;

        [SwaggerIgnore]

        public decimal? Weith { get; set; } = 0;


    }
}
