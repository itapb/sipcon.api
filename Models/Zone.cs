using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Swashbuckle.AspNetCore.Annotations;

namespace Models
{
    public class Zone:Record
    {
    
        [Required]
        public string? Name { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public Int32? WarehouseId { get; set; }

        [SwaggerIgnore]
        public string? WarehouseName { get; set; }

        [Required]
        public string? Size { get; set; } = string.Empty;
    }
}
