using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Swashbuckle.AspNetCore.Annotations;

namespace Models
{
    public class Assessment : Record
    {


        [SwaggerIgnore] public Int32? RecordId { get; set; }
        [Required] public Int32? Quantity { get; set; }
        [SwaggerIgnore] public Int32? ModuleId { get; set; }
        [SwaggerIgnore] public String? ModuleName { get; set; }
        [SwaggerIgnore] public String? UserDealerName { get; set; }
        [SwaggerIgnore] public String? UserDealerLastName { get; set; }
        [SwaggerIgnore] public String? UserSupplierName { get; set; }
        [SwaggerIgnore] public String? UserSupplierLastName { get; set; }
        [SwaggerIgnore] public DateTime? StartDate { get; set; }
        [SwaggerIgnore] public DateTime? EndDate { get; set; }



    }
}