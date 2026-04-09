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
        [SwaggerIgnore] public String? Vin { get; set; }
        [SwaggerIgnore] public String? Plate { get; set; }
        [SwaggerIgnore] public String? EngineSerial { get; set; }
        [SwaggerIgnore] public String? ModelName { get; set; }
        [SwaggerIgnore] public String? FirstName { get; set; }
        [SwaggerIgnore] public String? LastName { get; set; }
        [SwaggerIgnore] public String? Phone { get; set; }
        [SwaggerIgnore] public String? Email { get; set; }
        [SwaggerIgnore] public String? DealerName { get; set; }
        [SwaggerIgnore] public String? DealerCod { get; set; }
        [SwaggerIgnore] public String? SupplierCod { get; set; }
        [SwaggerIgnore] public String? SupplierName { get; set; }
        [SwaggerIgnore] public String? Description { get; set; }
        [SwaggerIgnore] public String? Year { get; set; }
        [SwaggerIgnore] public String? Color { get; set; }
        [SwaggerIgnore] public String? Direction { get; set; }
        [SwaggerIgnore] public String? BrandName { get; set; }
        [SwaggerIgnore] public Int32? BrandId { get; set; }
        [SwaggerIgnore] public Int32? SupplierId { get; set; }
        [SwaggerIgnore] public String? Seller { get; set; }
        [SwaggerIgnore] public String? CustomerCity { get; set; }
        [SwaggerIgnore] public String? DealerCity { get; set; }
        [SwaggerIgnore] public DateTime? DateCreated { get; set; }
        [SwaggerIgnore] public int? Topkm { get; set; }
        [SwaggerIgnore] public int? TopMonths { get; set; }
        [SwaggerIgnore] public int? Km { get; set; }
        [SwaggerIgnore] public int? Months { get; set; }
        [SwaggerIgnore] public int? TopYear { get; set; }
        [SwaggerIgnore] public DateTime? Birthday { get; set; } = DateTime.Now;
        [SwaggerIgnore] public String? Male { get; set; }
        [SwaggerIgnore] public String? Observation { get; set; }
        [SwaggerIgnore] public DateTime? UnlockDate { get; set; }

        [SwaggerIgnore] public String? CertificateNumber { get; set; }


    }
}
