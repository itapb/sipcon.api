using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Swashbuckle.AspNetCore.Annotations;

namespace Models
{
    public class Model:Record   

    {
        [Required]
        public Int32? BrandId { get; set; }

        [Required]
        public string? Description { get; set; }

        [Required]
        public string? Name { get; set; }

        [Required]
        public Int32? PolicyTypeId { get; set; }

        [SwaggerIgnore]
        public string? PolicyTypeName { get; set; }

        [SwaggerIgnore]
        public string? BrandName { get; set; }
        [SwaggerIgnore]
        public string? Code { get; set; }

        [SwaggerIgnore]
        public String? RowReference { get; set; }


    }
}
