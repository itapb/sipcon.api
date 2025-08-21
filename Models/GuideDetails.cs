using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class GuideDetails
    {
        //public int? GuideId { get; set; }
        public int? PartId { get; set; }
        public string? PartInnercode { get; set; }
        public string? PartDescription { get; set; }
        public int Quantity { get; set; }
        public string? PackageId { get; set; }
        public int? PackageNumber { get; set; }
        public string? PackageCode { get; set; }


    }
}
