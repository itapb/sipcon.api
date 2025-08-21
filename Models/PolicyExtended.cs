using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

namespace Models
{
    public class PolicyExtended : Policy
    {

        [SwaggerIgnore] public String? Vin { get; set; }
        [SwaggerIgnore] public String? Plate { get; set; }
        [SwaggerIgnore] public String? EngineSerial { get; set; }
        [SwaggerIgnore] public String? ModelName { get; set; }
        [SwaggerIgnore] public String? FirstName { get; set; }
        [SwaggerIgnore] public String? LastName { get; set; }
        [SwaggerIgnore] public String? Phone { get; set; }
        [SwaggerIgnore] public String? Email { get; set; }
        [SwaggerIgnore] public String? DealerName { get; set; }
        [SwaggerIgnore] public String? DealerCod { get; set; }
        [SwaggerIgnore] public String? SupplierCod { get; set; }
        [SwaggerIgnore] public String? SupplierName { get; set; }
        [SwaggerIgnore] public String? Description { get; set; }
        [SwaggerIgnore] public String? Year { get; set; }
        [SwaggerIgnore] public String? Color { get; set; }
        [SwaggerIgnore] public String? Direction { get; set; }
        [SwaggerIgnore] public String? BrandName { get; set; }
        [SwaggerIgnore] public Int32? BrandId { get; set; }
        [SwaggerIgnore] public Int32? SupplierId { get; set; }


    }
}
