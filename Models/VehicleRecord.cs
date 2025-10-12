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
        public List<ServiceRecord> serviceRecord { get; set; } = new List<ServiceRecord>();

        [SwaggerIgnore]
        public List<EstatusRecord> EstatusRecord { get; set; } = new List<EstatusRecord>();
        

    }
}
