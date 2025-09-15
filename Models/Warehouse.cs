using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Swashbuckle.AspNetCore.Annotations;

namespace Models
{
    public class Warehouse:Record
    {
 

        [Required]
        public string? Name { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public Int32? SupplierId { get; set; }

        [SwaggerIgnore]
        public string? SupplierName { get; set; }

        [SwaggerIgnore]
        public string? BrandName { get; set; }

        [SwaggerIgnore]
        public string? SupplierReference { get; set; }

        [Required]
        public bool? Sell { get; set; } = true;

        [SwaggerIgnore]
        public string? RowReference { get; set; }

    }
}
