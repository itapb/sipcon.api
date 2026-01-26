using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

namespace Models
{
    
    public class Dms: Record
    {
        [SwaggerIgnore] public String? Srg { get; set; }
        [SwaggerIgnore] public String? CodDms { get; set; }
        [SwaggerIgnore] public String? CodItem { get; set; }
        [SwaggerIgnore] public String? Description { get; set; }
        [SwaggerIgnore] public String? PreApproval { get; set; }
        [SwaggerIgnore] public Decimal? PaidAmount { get; set; }
        [SwaggerIgnore] public Decimal? BaseAmount { get; set; }
        [SwaggerIgnore] public int? SupplierId { get; set; }
        [SwaggerIgnore] public String? RowReference { get; set; }
        [SwaggerIgnore] public DateTime? DmsDate { get; set; }
        [SwaggerIgnore] public DateTime? PreApprovalDate { get; set; }
        [SwaggerIgnore] public DateTime? finalApprovalDate { get; set; }
        [SwaggerIgnore] public String? Estatus { get; set; }
        [SwaggerIgnore] public int? EstatusId { get; set; }
        [SwaggerIgnore] public Boolean? Completed { get; set; }


    }


    public class PaidDetailsDms
    {
        [SwaggerIgnore] public String? DmsId { get; set; }
        [SwaggerIgnore] public String? Date { get; set; }
        [SwaggerIgnore] public Decimal? Amount { get; set; }


    }




}