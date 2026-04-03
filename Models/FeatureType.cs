using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

namespace Models
{
    public class FeatureType : Record
    {
        [Required] public String? Name { get; set; }

        [Required] public Int32? FaseId { get; set; }

        [SwaggerIgnore] public String? FaseName { get; set; }
        [SwaggerIgnore] public String? DealerName {  get; set; }
        [SwaggerIgnore] public String? Brand {  get; set; }
    }
}