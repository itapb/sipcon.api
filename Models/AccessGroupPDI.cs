using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Swashbuckle.AspNetCore.Annotations;

namespace Models
{
    public class AccessGroupPDI

    {
        [SwaggerIgnore]
        public int? Id { get; set; }

        [SwaggerIgnore]
        public string? FaseName { get; set; } 

        [SwaggerIgnore]
        public int? AreaId { get; set; }
        
        [SwaggerIgnore]
        public string? AreaName { get; set; }

        [SwaggerIgnore]
        public bool? GivesOutCar { get; set; }
    }
}
