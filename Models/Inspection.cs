using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Swashbuckle.AspNetCore.Annotations;

namespace Models
{
    public class Inspection : Record
    {
        [Required]
        public int? CreatedBy { get; set; }

        [SwaggerIgnore]
        public String? NameUser { get; set; }

        [Required]
        public int? IdVehicle { get; set; }

        [SwaggerIgnore]
        public String? VehiclePlate { get; set; }

        [Required]
        public int? IdArea { get; set; }

        [SwaggerIgnore]
        public String? NameArea { get; set; }

        [Required]
        public DateTime? DateReceipt { get; set; }

        [Required]
        public DateTime? DateInspection { get; set; }

        [SwaggerIgnore]
        public DateTime? Created { get; set; }

        [SwaggerIgnore]
        public DateTime? Updated { get; set; }
    }
}