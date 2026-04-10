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

    public class InspectionFase : Record
    {
        [Required] public Int32? InspectionId { get; set; }
        [Required] public Int32? FaseId { get; set; }
        [SwaggerIgnore] public String? Fase { get; set; }
        [SwaggerIgnore] public DateTime? CompletedDate { get; set; }
        [SwaggerIgnore] public Int32? IsCompleted { get; set; }

        [SwaggerIgnore] public Int32? AreaId { get; set; }
        [SwaggerIgnore] public String? Area { get; set; }
    }

    public class FullInspection
    {
        [Required] public Int32? VehicleId { get; set; }
        [Required] public Int32? AreaId { get; set; }
    }
}