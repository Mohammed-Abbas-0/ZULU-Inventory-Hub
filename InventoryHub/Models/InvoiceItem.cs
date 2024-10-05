using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryHub.Models
{
    public class InvoiceItem
    {
        [Key]
        public int InvoiceItemId { get; set; } 
        public int InvoiceId { get; set; } 
        public Invoice Invoice { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; } 

        public int Quantity { get; set; } 
        public decimal UnitPrice { get; set; } 
        public decimal TotalPrice => Quantity * UnitPrice; 
    }

}
