using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryHub.Models
{
    public class SalesOrder
    {
        public int SalesOrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public string CustomerName { get; set; }
        public string Status { get; set; } // مثل: معلق، مكتمل
        public List<SalesOrderItem> Items { get; set; } = new List<SalesOrderItem>();

    }
}
