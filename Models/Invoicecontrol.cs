using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Swashbuckle.AspNetCore.Annotations;

    

namespace Models
{
    public class Invoicecontrol : Record
    {
        [SwaggerIgnore]
        public int? InvoiceId { get; set; }

        [SwaggerIgnore]
        public int? ControlId { get; set; }

        [SwaggerIgnore]
        public int? Invoiced { get; set; }

        [SwaggerIgnore]
        public int? Dispatched { get; set; }

        [Required]
        public int? Mark { get; set; }

        [SwaggerIgnore]
        public int? UserSinc { get; set; }

        [SwaggerIgnore]
        public DateTime? ControlDate { get; set; }


        [SwaggerIgnore]
        public DateTime? SincDate { get; set; }

        [SwaggerIgnore]
        public decimal? Price { get; set; }

        //CUSTOMER
        [SwaggerIgnore]
        public string? CustomerId { get; set; }
        [SwaggerIgnore]
        public string? Vat { get; set; }
        [SwaggerIgnore]
        public string? FiscalName { get; set; }

        //SUPPLIER  
        [SwaggerIgnore]
        public string? SupplierName { get; set; }

        //PART
        [SwaggerIgnore]
        public string? PartInnerCode { get; set; }
        [SwaggerIgnore]
        public string? PartName { get; set; }

        //SALE ORDER
        [SwaggerIgnore]
        public int? SaleOrderNumber { get; set; }

        //MOVEMENTDETAILS
        [SwaggerIgnore]
        public string? LocationName { get; set; }

        //PACKAGELIST
        [SwaggerIgnore]
        public int? Required { get; set; }

        [SwaggerIgnore]
        public int? BackOrderId { get; set; }

        [SwaggerIgnore]
        public int? MovementDetailId { get; set; }

        [SwaggerIgnore]
        public int? PartId { get; set; }

        [SwaggerIgnore]
        public int? SupplierId { get; set; }

        [SwaggerIgnore]
        public int? Pending { get; set; }

    }
}