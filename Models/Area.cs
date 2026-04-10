 using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Swashbuckle.AspNetCore.Annotations;

namespace Models
{
    public class Area:Record   

    { 
        [Required]
        public string? Name { get; set; }

        [Required]
        public int? SupplierId { get; set; }

        public int? DealerId { get; set; }

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
