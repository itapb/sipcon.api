using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class VehicleRecord
    {


        [SwaggerIgnore]
        public Models.VehicleFull Vehicle { get; set; } = new Models.VehicleFull();

        [SwaggerIgnore]
        public List<Companies> Suppliers { get; set; } = new List<Companies>();

        [SwaggerIgnore]
        public List<Companies> Dealers { get; set; } = new List<Companies>();
        
        [SwaggerIgnore]
        public List<Module> Modules { get; set; } = new List<Module>();

    }
}
