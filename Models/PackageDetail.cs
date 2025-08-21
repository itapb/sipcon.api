using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Swashbuckle.AspNetCore.Annotations;

namespace Models
{
    public class PackageDetail
    {
        
        public Int32? Id { get; set; }

        [Required]
        public Int32? PackageId { get; set; }

        [SwaggerIgnore]
        public string? PackageCode { get; set; }

        [Required]
        public Int32? PartId { get; set; }

        [SwaggerIgnore]
        public string? PartName { get; set; }

        [SwaggerIgnore]
        public string? PartInnerCode { get; set; }

        [SwaggerIgnore]
        public string? PartBarCode { get; set; }

        [Required]
        public Int32? Quantity { get; set; }

        [SwaggerIgnore]
        public Int32? Pending { get; set; }

    }
}
