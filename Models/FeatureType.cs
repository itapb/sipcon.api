using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

namespace Models
{
    public class FeatureType : Record
    {
        [Required] public String? Name { get; set; }
        [Required] public Int32? FaseId { get; set; }
        [SwaggerIgnore] public String? Fase { get; set; }
        [SwaggerIgnore] public Int32? AreaId { get; set; }
        [SwaggerIgnore] public String? Area { get; set; }
        [Required] public Int32? DealerId { get; set; }
        [SwaggerIgnore] public String? Dealer { get; set; }
        [Required] public Int32? SupplierId { get; set; }
        [SwaggerIgnore] public String? Supplier { get; set; }
        [SwaggerIgnore] public String? UpdatedBy { get; set; }
    }
}