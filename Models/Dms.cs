using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

namespace Models
{
    
    public class Dms: Record
    {
        [SwaggerIgnore] public String? Srg { get; set; }
        [SwaggerIgnore] public String? CodDms { get; set; }
        [SwaggerIgnore] public String? CodItem { get; set; }
        [SwaggerIgnore] public String? Description { get; set; }
        [SwaggerIgnore] public String? PreApproval { get; set; }
        [SwaggerIgnore] public String? PaidAmount { get; set; }
        [SwaggerIgnore] public Decimal? BaseAmount { get; set; }
        [SwaggerIgnore] public Decimal? SupplierId { get; set; }

    }


}