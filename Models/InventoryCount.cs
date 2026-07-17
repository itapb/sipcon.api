using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Swashbuckle.AspNetCore.Annotations;


namespace Models
{
    public class InventoryCount : Record
    {

       [Required] public String? Description { get; set; }
       [Required] public String? Type { get; set; }
       [Required] public Int32? SupplierId { get; set; }

    }

    public class GetInventoryCount : Record
    {

        [SwaggerIgnore] public String? Description { get; set; }
        [SwaggerIgnore] public int? TypeId { get; set; }
        [SwaggerIgnore] public String? Type { get; set; }
        [SwaggerIgnore] public Int32? SupplierId { get; set; }
        [SwaggerIgnore] public DateTime? Created { get; set; }
        [SwaggerIgnore] public int? StatusId { get; set; }
        [SwaggerIgnore] public int? UserId { get; set; }
        [SwaggerIgnore] public String? SupplierName { get; set; }
        [SwaggerIgnore] public DateTime? PreInventoryDate { get; set; }
        [SwaggerIgnore] public DateTime? InventoryDate { get; set; }
        [SwaggerIgnore] public DateTime? FinishDate { get; set; }
        [SwaggerIgnore] public String? StatusName { get; set; }
        [SwaggerIgnore] public String? UserName { get; set; }
    }
}
