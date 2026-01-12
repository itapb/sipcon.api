using System;
using System.Collections.Generic;
using System.Text;

namespace Models
{
    public class SaleOrderResume
    {
        public int? SaleOrderId { get; set; }
        public string? SaleOrderDate { get; set; }

        public string? SaleOrderType { get; set; }

        public bool? Paralyzed { get; set; }

        public string? SaleOrderVin { get; set; }

        public string? SaleOrderCustomer { get; set; }

        public string? DealerName { get; set; }

        public int? PartId { get; set; }
        public string? PartCode { get; set; }

        public string? PartName { get; set; }

        public decimal? Price { get; set; }


        public int? Required { get; set; }
        public int? BackOrder { get; set; }
        public int? Dismissed { get; set; }
        public int? Picking { get; set; }
        public int? Dispatched { get; set; }

        public int? Invoiced { get; set; }

        public int? Sent { get; set; }

        public int? Received { get; set; }


    }
}
