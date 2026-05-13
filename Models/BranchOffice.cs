using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

namespace Models
{
    public class BranchOffice : Record
    {
        [Required] public Int32? ContactId { get; set; }
        [Required] public String? Address { get; set; }

        [SwaggerIgnore] public String? Vat { get; set; }
        [SwaggerIgnore] public String? Name { get; set; }
        [SwaggerIgnore] public Int32? CreatedBy { get; set; }
        [SwaggerIgnore] public Int32? UpdatedBy { get; set; }
        [SwaggerIgnore] public DateTime? DateCreated { get; set; }
        [SwaggerIgnore] public DateTime? DateUpdated { get; set; }

    }
}
