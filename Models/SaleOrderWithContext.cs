using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class SaleOrderWithContext
    {
        public SaleOrder SaleOrder { get; set; } = new SaleOrder();

        public List<SaleOrderDetail> Details { get; set; } = new List<SaleOrderDetail>();

    }
}
