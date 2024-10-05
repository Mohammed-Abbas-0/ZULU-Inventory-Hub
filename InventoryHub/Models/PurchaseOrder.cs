using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryHub.Models
{
    public class PurchaseOrder
    {
        public int PurchaseOrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public string SupplierName { get; set; } // اسم المورد
        public string Status { get; set; } // مثل: معلق، مكتمل
        public List<PurchaseOrderItem> Items { get; set; } = new List<PurchaseOrderItem>();

    }
}
