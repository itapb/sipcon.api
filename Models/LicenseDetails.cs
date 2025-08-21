using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

namespace Models
{
    public class LicenseDetails: Record
    {
        [Required]
        [Range(1, int.MaxValue)]
        public Int32? LicenseId { get; set; }
        [SwaggerIgnore] public Int32? ReportId { get; set; }
        [Required] public String? Vin { get; set; }
        [SwaggerIgnore] public Int32? VehicleId { get; set; }
        [SwaggerIgnore] public String? LicenseName { get; set; }
        [SwaggerIgnore] public String? EstatusVehicle { get; set; }
        [SwaggerIgnore] public Int32? EstatusDetailId { get; set; }
        [SwaggerIgnore] public String? EstatusDetail { get; set; }
        [SwaggerIgnore] public String? Model { get; set; }
        [SwaggerIgnore] public Int32? Year { get; set; }
        [SwaggerIgnore] public String? CustomerId { get; set; }
        [SwaggerIgnore] public String? CustomerName { get; set; }
        [SwaggerIgnore] public String? CustomerLastName { get; set; }
        [SwaggerIgnore] public Int32? DealerId { get; set; }
        [SwaggerIgnore] public String? DealerName { get; set; }
    }
}