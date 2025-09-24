using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

namespace Models
{
    public class ServiceMaintenance : Record
    {
        //Datos de Servicio
        [SwaggerIgnore] public Int32? ServiceTypeId { get; set; } = 1;
        [SwaggerIgnore] public String? ServiceTypeName { get; set; }
        [SwaggerIgnore] public Int32? ReportTypeId { get; set; } = 1;
        [SwaggerIgnore] public String? ReportTypeName { get; set; }
        [Required] public  String? OrderNumber { get; set; }
        [Required] public DateTime? ServiceDate { get; set; }
        [Required] public String? DealerReport { get; set; }
        [SwaggerIgnore] public String? SupplierReport { get; set; }
        [Required] public Int32? PolicyDetailId { get; set; }
        [Required] public Int32? Km { get; set; }


        [Required] public Int32? DealerId { get; set; }
        [SwaggerIgnore] public String? DealerServiceName { get; set; }
        [SwaggerIgnore] public String? DealerServiceCod { get; set; }
        [SwaggerIgnore] public String? AuthorizedUserName { get; set; }
        [SwaggerIgnore] public String? AuthorizedUserLastName { get; set; }
        [SwaggerIgnore] public String? DealerAddress { get; set; }
        [SwaggerIgnore] public String? DealerServiceCity { get; set; }
        [SwaggerIgnore] public String? DealerServiceState { get; set; }
        [SwaggerIgnore] public String? SrgNumber { get; set; }


        //Datos de Vehiculo
        [SwaggerIgnore] public String? DealerSaleId { get; set; }
        [SwaggerIgnore] public String? DealerSaleName { get; set; }
        [SwaggerIgnore] public String? DealerSaleCod { get; set; }
        [SwaggerIgnore] public String? NumberPolicy { get; set; }
        [Required] [Range(1, int.MaxValue)] public Int32? VehicleId { get; set; }
     
        [SwaggerIgnore] public String? Plate { get; set; }
        [SwaggerIgnore] public String? Vin { get; set; }
        [SwaggerIgnore] public Int32? ModelId { get; set; }
        [SwaggerIgnore] public String? ModelName{ get; set; }
        [SwaggerIgnore] public String? Year { get; set; }
        
        // Datos de Cliente
        [Required] [Range(1, int.MaxValue)] public Int32? CustomerId { get; set; }
        [SwaggerIgnore] public String? Vat { get; set; }
        [SwaggerIgnore] public String? CustomerName { get; set; }
        [SwaggerIgnore] public String? CustomerLastName { get; set; }
        [SwaggerIgnore] public String? CustomerPhone { get; set; }
        
        // Datos de Facturacion
        [Required] public String? InvoiceNumber { get; set; }
        [SwaggerIgnore] public decimal? InvoiceAmount { get; set; }
        [Required] public DateTime? InvoiceDate { get; set; }
        
        //Estatus
        [SwaggerIgnore][Range(1, 22)] public Int32? EstatusId { get; set; }
        [SwaggerIgnore] public String? EstatusName { get; set; }
       

    }
}