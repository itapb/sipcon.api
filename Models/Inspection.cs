using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

namespace Models
{
    public class Inspection : Record
    {
        [Required] public int? CreatedBy { get; set; }
        [Required] public int? VehicleId { get; set; }
        [Required] public int? AreaId { get; set; }

        public int? InitBy { get; set; }
        public int? ClosedBy { get; set; }
        public int? TransporterId { get; set; }
        public int? RecepBy { get; set; }
        public string? Comment { get; set; }
        public int? DealerId { get; set; }

        [SwaggerIgnore] public String? UserName { get; set; }
        [SwaggerIgnore] public String? InitByName { get; set; }
        [SwaggerIgnore] public String? ClosedByName { get; set; }
        [SwaggerIgnore] public String? TransporterName { get; set; }
        [SwaggerIgnore] public String? RecepByName { get; set; }

        [SwaggerIgnore] public String? VehiclePlate { get; set; }
        [SwaggerIgnore] public String? Model { get; set; }
        [SwaggerIgnore] public String? Lote { get; set; }
        [SwaggerIgnore] public String? Vin { get; set; }
        [SwaggerIgnore] public String? NameArea { get; set; }
        [SwaggerIgnore] public DateTime? Created { get; set; }

        public DateTime? DInit { get; set; }
        public DateTime? DClose { get; set; }
        public DateTime? DReception { get; set; }

        [SwaggerIgnore] public Int32? IsCompleted { get; set; }
    }

    public class InspectionFase : Record
    {
        [Required] public Int32? InspectionId { get; set; }
        [Required] public Int32? FaseId { get; set; }
        [SwaggerIgnore] public String? Fase { get; set; }
        [SwaggerIgnore] public DateTime? CompletedDate { get; set; }
        [SwaggerIgnore] public Int32? IsCompleted { get; set; }
        [SwaggerIgnore] public String? InitDate { get; set; }
        [SwaggerIgnore] public Int32? AreaId { get; set; }
        [SwaggerIgnore] public String? Area { get; set; }
        [SwaggerIgnore] public Int32? UserInitId { get; set; }
        [SwaggerIgnore] public String? Login { get; set; } 
        [SwaggerIgnore] public Int32? Completed { get; set; }
    }

    public class FullInspection
    {
        [Required] public String? Identifier { get; set; }
        [Required] public Int32? AreaId { get; set; }
    }

    public class InspectionDetail : Record
    {
        [Required] public Int32? Value { get; set; }
        [Required] public String? Observation { get; set; }
        [SwaggerIgnore] public String? FileUrl { get; set; }
        [Required] public Int32? InspectionId { get; set; }
        [Required] public Int32? FeatureId { get; set; }
        [SwaggerIgnore] public String? Feature { get; set; }
        [SwaggerIgnore] public Int32? FeatureTypeId { get; set; }
        [SwaggerIgnore] public String? FeatureValueTypeId { get; set; }
        [SwaggerIgnore] public String? FeatureType { get; set; }
        [SwaggerIgnore] public String? FaseId { get; set; }
        [SwaggerIgnore] public String? Fase { get; set; }
        [SwaggerIgnore] public Int32? AreaId { get; set; }
        [SwaggerIgnore] public String? Area { get; set; }
        [SwaggerIgnore] public String? Color { get; set; }
        [SwaggerIgnore] public String? Model { get; set; }
        [SwaggerIgnore] public String? Vin { get; set; }
        [SwaggerIgnore] public String? Plate { get; set; }
    }

    public class DealersByInspection
    {
        [SwaggerIgnore] public Int32? InspectionId { get; set; }
        [SwaggerIgnore] public Int32? VehicleId { get; set; }
        [SwaggerIgnore] public Int32? DealerId { get; set; }
        [SwaggerIgnore] public String? DealerName { get; set; }

    }
}