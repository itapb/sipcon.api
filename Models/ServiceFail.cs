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
        [SwaggerIgnore] public new Decimal? InvoiceAmount { get; set; }
    }

}