using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

namespace Models
{
    public class ServiceAssistance : Record
    {
        //Datos de Servicio
        [SwaggerIgnore] public Int32? ServiceTypeId { get; set; } = 2;
        [SwaggerIgnore] public Int32? ReportTypeId { get; set; } = 1;
        [SwaggerIgnore] public String? ServiceTypeName { get; set; }
        [Required] public String? OrderNumber { get; set; }
        [Required] public DateTime? ServiceDate { get; set; }
        [Required] public String? CustomerReport { get; set; }
        [Required] public String? DealerReport { get; set; }
        [SwaggerIgnore] public String? TechnicalSolution { get; set; }
        [SwaggerIgnore] public String? SupplierReport { get; set; }
        [Required] public Int32? Km { get; set; }
        public bool? Paralyzed { get; set; }
        [SwaggerIgnore] public Int32? AssitenanceTypeId { get; set; }
        [SwaggerIgnore] public String? AssitenanceType { get; set; }
        public Int32? PossibleFaultId { get; set; }
        [SwaggerIgnore] public String? PossibleFault { get; set; }

        [SwaggerIgnore] public DateTime? StartDate { get; set; }

        [SwaggerIgnore] public DateTime? FinishDate { get; set; }

        [SwaggerIgnore] public Int32? Assesment { get; set; }

        [SwaggerIgnore] public String? AuthorizedUserName { get; set; }
        [SwaggerIgnore] public String? AuthorizedUserLastName { get; set; }






        [Required] public Int32? DealerId { get; set; }
        [Required] public Int32? CustomerId { get; set; }
        [SwaggerIgnore] public String? DealerServiceName { get; set; }
        [SwaggerIgnore] public String? DealerServiceCod { get; set; }
 

        //Datos de Vehiculo
        [Required] [Range(1, int.MaxValue)] public Int32? VehicleId { get; set; }
        [SwaggerIgnore] public String? NumberPolicy { get; set; }
  
        [SwaggerIgnore] public String? Plate { get; set; }
        [SwaggerIgnore] public String? Vin { get; set; }
        [SwaggerIgnore] public Int32? ModelId { get; set; }
        [SwaggerIgnore] public String? ModelName{ get; set; }
        [SwaggerIgnore] public String? Year { get; set; }   
        //Estatus
        [SwaggerIgnore][Range(1, 22)] public Int32? EstatusId { get; set; }
        [SwaggerIgnore] public String? EstatusName { get; set; }
       

    }
}