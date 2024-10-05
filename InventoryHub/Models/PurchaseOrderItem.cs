using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryHub.Models
{
    public class PurchaseOrderItem
    {
        public int PurchaseOrderItemId { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public int Quantity { get; set; } // الكمية المشتراة
    }
}
