using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class VehicleRecord
    {


        [SwaggerIgnore]
        public Models.Vehicle Vehicle { get; set; } = new Models.Vehicle();

        [SwaggerIgnore]
        public Models.CustomerVehicleRecord Customer { get; set; } = new Models.CustomerVehicleRecord();

        [SwaggerIgnore]
        public Models.policyVehicleRecord Policy { get; set; } = new Models.policyVehicleRecord();

        [SwaggerIgnore]
        public List<ServiceRecord> ServiceRecord { get; set; } = new List<ServiceRecord>();

        [SwaggerIgnore]
        public List<EstatusRecord> EstatusRecord { get; set; } = new List<EstatusRecord>();
        

    }

    public class CustomerVehicleRecord
    {
        [SwaggerIgnore]  public Int32? CustomerId { get; set; }
        [SwaggerIgnore] public string? CustomerName { get; set; }
        [SwaggerIgnore] public String? CustomerLastName { get; set; }
        [SwaggerIgnore] public String? Phone { get; set; }
        [SwaggerIgnore] public String? Email { get; set; }
        [SwaggerIgnore] public String? Direction { get; set; }
        [SwaggerIgnore] public String? Vat { get; set; }



    }

    public class policyVehicleRecord
    {

        [SwaggerIgnore] public Int32? PolicyId { get; set; }
        [SwaggerIgnore] public String? Number { get; set; }
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
