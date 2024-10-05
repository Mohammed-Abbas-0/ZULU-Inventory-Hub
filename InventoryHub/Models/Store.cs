using InventoryHub.ServicesPatterns.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace InventoryHub.Models
{
    public class Store:AuditableEntity
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Store Name is Required")]
        public string StoreName { get; set; }
        [Required(ErrorMessage = "Address is Required")]
        public string Address { get; set; }
        
        [JsonIgnore]
        public ICollection<Product> Products { get; set; }
    }
}
