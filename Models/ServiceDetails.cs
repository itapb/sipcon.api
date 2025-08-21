using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Swashbuckle.AspNetCore.Annotations;


namespace Models
{
    public class ServiceDetails : Record
    {

        [Required] public Int32? ServiceId { get; set; }
        [SwaggerIgnore] public String? ServiceName { get; set; }
        [Required] public Int32? ItemId { get; set; }
        [SwaggerIgnore] public String? ItemName { get; set; }
        [Required] [StringLength(1, MinimumLength = 1)] public String? Type { get; set; }
        [Required] public  Int32? Quantity { get; set; }
        [Required] public Decimal? UnitPrice { get; set; }
        [Required] public Boolean? IsExternal { get; set; }
        [Required] public Boolean? IsTax { get; set;}
        [SwaggerIgnore] public Decimal? Price { get; set; }
        [SwaggerIgnore] public String? Serial { get; set; }
        [SwaggerIgnore] public String? Reference { get; set; }
        public String? InvoiceNumber { get; set; }

        //Estatus
        [SwaggerIgnore][Range(1, 22)] public Int32? EstatusId { get; set; }
        [SwaggerIgnore] public String? EstatusName { get; set; }

    }
}
