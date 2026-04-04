using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

namespace Models
{
    public class Inspection : Record
    {
        [Required] public int? CreatedBy { get; set; }
        [Required] public int? VehicleId { get; set; }
        [Required] public int? AreaId { get; set; }
        [SwaggerIgnore] public String? UserName { get; set; }
        [SwaggerIgnore] public String? VehiclePlate { get; set; }
        [SwaggerIgnore] public String? NameArea { get; set; }
        [SwaggerIgnore] public DateTime? Created { get; set; }
        [SwaggerIgnore] public DateTime? Updated { get; set; }
    }
}