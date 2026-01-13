using Models;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Util;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Models
{
    public class ClaimPart 
    {
        [Required]
        public int? Id { get; set; }  
        [Required]
        public int? SupplierId { get; set; } 
        [Required]
        public int? DealerId { get; set; } 
        [Required]
        public int? UserId { get; set; }

        public string? Reference { get; set; }  
        public string? Note { get; set; } 
        public string? Comment { get; set; } 
        public string? Invoice { get; set; } 
        public string? Guide { get; set; }

        [SwaggerIgnore]
        public string? DealerName { get; set; }  
        [SwaggerIgnore]
        public string? SupplierName { get; set; }  
        [SwaggerIgnore]
        public string? StatusName { get; set; }
        [SwaggerIgnore]
        public string? Login { get; set; }
        [SwaggerIgnore]
        public string? FiscalName { get; set; }  
        [SwaggerIgnore]
        public DateTime? DCreated { get; set; }
        [SwaggerIgnore]
        public DateTime? DUpdated { get; set; }

    }

} 