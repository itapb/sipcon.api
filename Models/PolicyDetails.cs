using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel;

namespace Models
{
    public class PolicyDetails : Record
    {
        [SwaggerIgnore] public Int32? Km { get; set; }
        [SwaggerIgnore] public Int32? FromKm { get; set; }
        [SwaggerIgnore] public Int32? UpToKm { get; set; }
        [SwaggerIgnore] public DateTime? Date { get; set; }
        [SwaggerIgnore] public DateTime? FromDate { get; set; }
        [SwaggerIgnore] public DateTime? UpToDate { get; set; }
        [SwaggerIgnore] public String? Valid { get; set; }
        [SwaggerIgnore] public Int32?  ServiceId { get; set; }



    }
}