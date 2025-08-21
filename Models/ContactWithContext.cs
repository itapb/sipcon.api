using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class ContactWithContext
    {
        public Contact Contact { get; set; }= new Contact();
        public List<Brand> Brands { get; set; } = new List<Brand>();
        public List<Related> Relateds { get; set; } = new List<Related>();
        public List<City> Cities { get; set; } = new List<City>();
        public List<PayMethod> PayMethods { get; set; } = new List<PayMethod>();
        public List<Group> Groups { get; set; } = new List<Group>();


    }
}
