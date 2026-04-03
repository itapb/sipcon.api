using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

namespace Models
{
    public class Fase:Record
    {
        [Required] public String? Name { get; set; }
        [Required] public int? OrderBy { get; set; }
        [Required] public int? AreaId { get; set; }
        [SwaggerIgnore] public String? AreaName { get; set; }
        [SwaggerIgnore] public String? DealerName { get; set; }
        [SwaggerIgnore] public String? SupplierName { get; set; }
        [SwaggerIgnore] public String? Brand { get; set; }
  }
}
