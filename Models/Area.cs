using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

namespace Models
{
    public class Area : Record
    {
        [Required] public String? Name { get; set; }
        [Required] public Int32? DealerId { get; set; }
        [SwaggerIgnore] public String ? DealerName { get; set; }
        [SwaggerIgnore] public String? SupplierName { get; set; }
        [SwaggerIgnore] public String? Brand { get; set; }
    }
}
