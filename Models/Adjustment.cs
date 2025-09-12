using System;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography.Pkcs;
using Swashbuckle.AspNetCore.Annotations;
using Util;

namespace Models
{
    public class Adjustment: Record
    { 

        [Required]
        public int? SupplierId { get; set; }
         
        [Required]
        public int? UserId { get; set; }
        
        [Required]
        public int? StatusId { get; set; }

        [SwaggerIgnore]
        public string? StatusName { get; set; }

        [SwaggerIgnore]
        public DateTime? DCreated { get; set; }

        [SwaggerIgnore]
        public string? UserLogin { get; set; }

        [SwaggerIgnore]
        public string? SupplierName { get; set; }
    }
} 
 