using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Swashbuckle.AspNetCore.Annotations;

namespace Models
{
    public class PowerBI_Reports : Record   
    {
        [SwaggerIgnore] public string? Report { get; set; }
        [SwaggerIgnore] public string? ReportId { get; set; }
        [SwaggerIgnore] public string? WorkSpace { get; set; }
        [SwaggerIgnore] public string? WorkSpaceId { get; set; }
    }

    public class PowerBI_Token
    {
        [SwaggerIgnore] public string? Token { get; set; }
    }

    public class ResponseToken
    {
       [SwaggerIgnore] public string? access_token { get; set; }
    }
}
