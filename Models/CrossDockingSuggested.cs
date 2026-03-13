using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class CrossDockingSuggested
    {
        public string? PartCode { get; set; }
        public string? PartDescription { get; set; }
        public int RequestedQuantity { get; set; }
    }
}