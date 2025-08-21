using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

namespace Models
{
    public class License: Record
    {
        [Required]
        [Range(1, int.MaxValue)]
        public Int32? SupplierId { get; set; }
        [Required] public String? Description { get; set; }
        [Required] public Int32? TypeId { get; set; }
        [Required] public DateTime? ExpirationDate { get; set; }
        [SwaggerIgnore] public String? Type { get; set; }
        [SwaggerIgnore] public String? EstatusName { get; set; }
        [SwaggerIgnore] public Int32? EstatusId { get; set; }
        

    }
}