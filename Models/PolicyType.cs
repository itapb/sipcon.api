using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

namespace Models
{
    public class PolicyType : Record
    {
        [Required]
        [Range(1, int.MaxValue)]
        public Int32? SupplierId { get; set; }

        [Required] public String? Description { get; set; }

        [Required]
        public Int32? Km { get; set; }

        [Required]
        public Int32? GapKm { get; set; }

        [Required]
        public Int32? TopKm { get; set; }

        [Required]
        public Int32? Months { get; set; }

        [Required]
        public Int32? GapMonths { get; set; }

        [Required]
        public Int32? TopMonths { get; set; }


        [Required]
        [Range(1, int.MaxValue)]
        public Int32? BrandId { get; set; }

        [SwaggerIgnore]
        public String? BrandName { get; set; }

        [SwaggerIgnore]
        public String? RowReference { get; set; }
        
        [SwaggerIgnore]
        public String? Supplier { get; set; }



    }
}