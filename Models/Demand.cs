using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Demand

    {
        [Required] public Int32? PartId { get; set; }
        [Required] public String InnerCode { get; set; }= string.Empty;
        [Required] public String? Description { get; set; }= string.Empty;
        [Required] public Int32? Quantity { get; set; }
        [Required] public Int32? DealerId { get; set; }
        [Required] public Int32? ModelId { get; set; }
        [Required] public Int32? ReasonId { get; set; }
        [SwaggerIgnore] public Int32? Id { get; set; }
        [SwaggerIgnore] public DateTime? Created { get; set; }
    }
}
