using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

namespace Models
{
    public class ServiceFail : Service
    {
        [SwaggerIgnore] public new Int32? ServiceTypeId { get; set; } = 3;
        [SwaggerIgnore] public new Int32? PolicyDetailId { get; set; }
        [SwaggerIgnore] public  Decimal? Tax { get; set; }
        [SwaggerIgnore] public Decimal? TaxBase { get; set; }
        [SwaggerIgnore] public Decimal? Exempt { get; set; }
        [SwaggerIgnore] public DateTime? AuthotizationDate { get; set; }
        [SwaggerIgnore] public Int32? SupplierId { get; set; }
        [SwaggerIgnore] public Int32? BrandId { get; set; }
        [SwaggerIgnore] public new Decimal? InvoiceAmount { get; set; }
    }

}