using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class SrgPending 
    {

        [Required]
        public int? Id { get; set; }

        [SwaggerIgnore]
        public string? Srg { get; set; }
        
        [SwaggerIgnore]
        public string? Vin { get; set; }

        [SwaggerIgnore]
        public string? Model { get; set; }

        [SwaggerIgnore]
        public string? DescriptionFail { get; set; }

        [SwaggerIgnore]
        public string? Invoice { get; set; }

        [SwaggerIgnore]
        public DateTime? InvoiceDate { get; set; }

        [SwaggerIgnore]
        public decimal? Exent { get; set; }

        [SwaggerIgnore]
        public decimal? TaxBase { get; set; }

        [SwaggerIgnore]
        public decimal? Mount { get; set; }

        [SwaggerIgnore]
        public decimal? Tax { get; set; }

        [SwaggerIgnore]

        public string? Dealer { get; set; }

        [SwaggerIgnore]

        public int? SupplierId { get; set; }

        [SwaggerIgnore]

        public int? BrandId { get; set; }
        
        [SwaggerIgnore]

        public String? Reporttype { get; set; }


    }
}
