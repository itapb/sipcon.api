using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Swashbuckle.AspNetCore.Annotations;
using Util;

namespace Models
{
    public class PackageList
    {
        [SwaggerIgnore]
        public Int32? Id { get; set; }

        [SwaggerIgnore]
        public string? Code { get; set; }

        [SwaggerIgnore]
        public Int32? SupplierId { get; set; }

        [SwaggerIgnore]
        public Int32? CustomerId { get; set; }

        [SwaggerIgnore]
        public string? SupplierName { get; set; }

        [SwaggerIgnore]
        public string? CustomerName { get; set; }   

        [SwaggerIgnore]
        public string? PartInnerCode { get; set; }

        [SwaggerIgnore]
        public string? PartName { get; set; }


        [SwaggerIgnore]
        public Int32? Quantity { get; set; }

    }
}
 