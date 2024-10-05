using InventoryHub.ServicesPatterns.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryHub.Models
{
    public enum InvoiceStatus
    {
        Unpaid = 1,   
        Paid,     
        Overdue  
    }

    public class Invoice: AuditableEntity
    {
        public int InvoiceId { get; set; }
        public string InvoiceNumber { get; set; }
        public int CustomerId { get; set; } 
        public Customer Customer { get; set; } 
        public DateTime? DueDate { get; set; } 
        public decimal TotalAmount { get; set; }
        public decimal PaidAmount { get; set; } 
        public decimal RemainingAmount => TotalAmount - PaidAmount;
        public InvoiceStatus Status { get; set; }

        public ICollection<InvoiceItem> Items { get; set; } 
    }
}
