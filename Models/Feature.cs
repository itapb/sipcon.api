using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

namespace Models
{
    public class Feature : Record
    {
        [Required] public String? Name { get; set; }

        [Required] public Int32? FeatureTypeId { get; set; }

        [Required] public int? ModelId { get; set; }

        [SwaggerIgnore] public String? Model { get; set; }

        [SwaggerIgnore] public String? FeatureType { get; set; }

        [SwaggerIgnore] public Int32? FaseId {  get; set; }

        [SwaggerIgnore] public String? Fase {  get; set; }

        [SwaggerIgnore] public Int32? AreaId { get; set; }

        [SwaggerIgnore] public String? Area { get; set; }

        [SwaggerIgnore] public String? DealerName { get; set; }

        [SwaggerIgnore] public String? SupplierName { get; set; }

        [SwaggerIgnore] public String? Brand { get; set; }
    }
}