using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

namespace Models
{
    public class VehicleFull: Vehicle
    {

        [SwaggerIgnore] public String? CustomerLastName { get; set; }
        [SwaggerIgnore] public String? Phone { get; set; }
        [SwaggerIgnore] public String? Email { get; set; }
        [SwaggerIgnore] public String? Direction { get; set; }

        //DATOS DE POLIZA
        [SwaggerIgnore] public Int32? PolicyId { get; set; }
        [SwaggerIgnore] public String? Number { get; set; }
        [SwaggerIgnore] public String? Vat { get; set; }
        [SwaggerIgnore] public DateTime? ActivationDate { get; set; }
        [SwaggerIgnore] public DateTime? LockDate { get; set; }
        [SwaggerIgnore] public DateTime? ExpirationDate { get; set; }
        [SwaggerIgnore] public String? InvoiceNumber { get; set; }
        [SwaggerIgnore] public decimal? InvoiceAmount { get; set; }
        [SwaggerIgnore] public DateTime? InvoiceDate { get; set; }
        [SwaggerIgnore] public Int32? PayMethodId { get; set; }
        [SwaggerIgnore][Range(1, 22)] public Int32? EstatusPolicyId { get; set; }
        [SwaggerIgnore] public String? EstatusPolicyName { get; set; }

    }
}
