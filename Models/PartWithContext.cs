using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class PartWithContext
    {

        public Part Part { get; set; } = new Part();
        public List<PartType> Types { get; set; } = new List<PartType>();
        public List<Family> Families { get; set; } = new List<Family>();
        public List<SubFamily> SubFamilies { get; set; } = new List<SubFamily>();
        public List<Tax> Taxes { get; set; } = new List<Tax>();
        public List<Um> Ums { get; set; } = new List<Um>();
        public List<Brand> Brands { get; set; } = new List<Brand>();
        public List<Contact> Suppliers { get; set; } = new List<Contact>();
        public List<RelatedModel> Models { get; set; } = new List<RelatedModel>();

    }
}
