using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Swashbuckle.AspNetCore.Annotations;


namespace Models
{
    public class Movement: Record
    {

        [Required]
        [Range(1, int.MaxValue)]
        public Int32? SupplierId { get; set; }

        [Required]
        public string? TypeId { get; set; } = string.Empty;

        public string Comment { get; set; } = string.Empty;
        
        [SwaggerIgnore]
        public string? GuideNumber { get; set; } = string.Empty;

        [SwaggerIgnore]
        public DateTime Created { get; set; } = DateTime.Now;

        [SwaggerIgnore]
        public string TypeName { get; set; } = string.Empty;

        [SwaggerIgnore]
        public string StatusName { get; set; } = string.Empty;

        [SwaggerIgnore]
        public string SupplierReference{ get; set; } = string.Empty;

        [SwaggerIgnore]
        public string SupplierName { get; set; } = string.Empty;

        [SwaggerIgnore]
        public string ManagerName { get; set; } = string.Empty;

        [SwaggerIgnore]
        public string ManagerLastName { get; set; } = string.Empty;

        [SwaggerIgnore]
        public string ContactName { get; set; } = string.Empty;


        public string Reference { get; set; } = string.Empty;

        [SwaggerIgnore]
        public string CreatedBy { get; set; } = string.Empty;

        [SwaggerIgnore]
        public string AssignTo { get; set; } = string.Empty;

        [Required]
        [Range(0, int.MaxValue)]
        public Int32? ContactId { get; set; } = 0;

        


    }
}
