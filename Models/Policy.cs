using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

namespace Models
{
    public class Policy : Record
    {
        [SwaggerIgnore] public String? Number { get; set; }
        [SwaggerIgnore] public Int32? PolicyTypeId { get; set; }
        [Required]
        [Range(1, int.MaxValue)]
        public Int32? VehicleId { get; set; }
        [Required]
        [Range(1, int.MaxValue)]
        public Int32? CustomerId { get; set; }
        [SwaggerIgnore] public String? Vat { get; set; }
        [SwaggerIgnore] public DateTime? ActivationDate { get; set; }
        [SwaggerIgnore] public DateTime? LockDate { get; set; }
        [SwaggerIgnore] public DateTime? ExpirationDate { get; set; }
        [Required] public String? InvoiceNumber { get; set; }
        [Required] public decimal? InvoiceAmount { get; set; }
        [Required] public DateTime? InvoiceDate { get; set; }
        [Required] public Int32? PayMethodId { get; set; }
        [SwaggerIgnore][Range(1, 22)] public Int32? EstatusId { get; set; }
        [SwaggerIgnore] public String? EstatusName { get; set; }
  

    }
}
