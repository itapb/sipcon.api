using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Swashbuckle.AspNetCore.Annotations;


namespace Models
{
    public class InventoryCount 
    {
        [Required] public int? Id { get; set; }
        [Required] public String? Description { get; set; }
       [Required] public int? TypeId { get; set; }
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


    public class CountAssign
    {
        [Required] public int? UserId { get; set; }
        [Required] public String? Assign { get; set; }
        [Required] public Int32? CountInventoryId { get; set; }
        [Required] public String? AssignType { get; set; }
        

    }

    public class GetInventoryCountDetail : Record
    {

        [SwaggerIgnore] public int? InventoryId { get; set; }
        [SwaggerIgnore] public int? CountId { get; set; }
        [SwaggerIgnore] public int? LocationId { get; set; }
        [SwaggerIgnore] public int? PartId { get; set; }
        [SwaggerIgnore] public int? QuantityOld { get; set; }
        [SwaggerIgnore] public int? QuantityNew { get; set; }
        [SwaggerIgnore] public int? UserId { get; set; }
        [SwaggerIgnore] public int? StatusId { get; set; }
        [SwaggerIgnore] public DateTime? Created { get; set; }
        [SwaggerIgnore] public DateTime? CountDate { get; set; }
        [SwaggerIgnore] public String? StatusName { get; set; }
        [SwaggerIgnore] public String? UserName { get; set; }
        [SwaggerIgnore] public String? Location { get; set; }
        [SwaggerIgnore] public String? Zone { get; set; }
        [SwaggerIgnore] public String? InnerCode { get; set; }
        [SwaggerIgnore] public String? PartName { get; set; }
        [SwaggerIgnore] public int? ZoneId { get; set; }
        [SwaggerIgnore] public int? Diference { get; set; }

    }


    public class CountSummary
    {
        [SwaggerIgnore] public int? ZoneId { get; set; }
        [SwaggerIgnore] public String? Zone { get; set; }
        [SwaggerIgnore] public Int32? LocationTotal { get; set; }
        [SwaggerIgnore] public Int32? LocationCounted { get; set; }
        [SwaggerIgnore] public Decimal? Porcentage { get; set; }
        [SwaggerIgnore] public String? UsersAssigned { get; set; }

    }


    public class CountType: Record
    {
        [Required] public String? Name { get; set; }
    }


    public class GetCountFull : CountSummary
    {

        public List<GetInventoryCountDetail>? InventoryCountDetail { get; set; } = new List<GetInventoryCountDetail>();

    }

}
