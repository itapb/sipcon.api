using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Package
    {

        [Required]
        public Int32? Id { get; set; }

        [Required]
        public Int32? SupplierId { get; set; }

        [Required]
        public Int32? CustomerId { get; set; }

        public Int32? GuideId { get; set; }

        public Int32? Number { get; set; }

        public string? Code { get; set; }

        public decimal? Weight { get; set; }

        public bool? Closed { get; set; }= false;



    }
}
