using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

namespace Models
{
    public class ServiceRecord 
    {
        //Datos de Servicio
        [Required] public int? ReportId { get; set; }
        [Required] public Int32? ServiceTypeId { get; set; }
        [SwaggerIgnore] public String? ServiceTypeName { get; set; }
        [Required] public Int32? ReportTypeId { get; set; }
        [SwaggerIgnore] public String? ReportTypeName { get; set; }
        [Required] public String? OrderNumber { get; set; }
        [Required] public DateTime? ServiceDate { get; set; }
     
       
        [SwaggerIgnore] public String? DealerServiceName { get; set; }
        [SwaggerIgnore] public String? DealerServiceCod { get; set; }
        [SwaggerIgnore] public String? SrgNumber { get; set; }
        [SwaggerIgnore] public Int32? Km { get; set; }

        [SwaggerIgnore] public decimal? InvoiceAmount { get; set; }

        [SwaggerIgnore][Range(1, 22)] public Int32? EstatusId { get; set; }
        [SwaggerIgnore] public String? EstatusName { get; set; }

        [SwaggerIgnore] public DateTime? Date { get; set; }



    }
}