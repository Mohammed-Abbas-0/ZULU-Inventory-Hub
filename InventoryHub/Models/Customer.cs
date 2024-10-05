using InventoryHub.ServicesPatterns.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryHub.Models
{
    public class Customer: AuditableEntity
    {
        [Key]
        public int CustomerId { get; set; } 
        [Required(ErrorMessage ="First Name  is Required.")]
        public string FirstName { get; set; } 
        [Required(ErrorMessage ="Last Name  is Required.")]
        public string LastName { get; set; }
        [Required(ErrorMessage ="Last Name  is Required.")]
        [EmailAddress]
        public string Email { get; set; }
        [Phone]
        public string PhoneNumber { get; set; } 
        public string Address { get; set; } 
        public bool IsActive { get; set; } 
        public string CustomerImage { get; set; }

        // علاقات
        public ICollection<Order> Orders { get; set; } // قائمة الطلبات المرتبطة بالعميل
        public ICollection<Invoice> Invoices { get; set; } // قائمة الفواتير الخاصة بالعميل
    }
}
