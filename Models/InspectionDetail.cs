using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

namespace Models
{
    public class InspectionDetail : Record
    {
        [Required] public bool? Value { get; set; }
        [Required] public String? Observation { get; set; }
        [SwaggerIgnore] public String? FileUrl { get; set; }
        [Required] public Int32? InspectionId { get; set; }
        [Required] public Int32? FeatureId { get; set; }
        [SwaggerIgnore] public String? Feature { get; set; }
        [SwaggerIgnore] public Int32? FeatureTypeId { get; set; }
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
}