using InventoryHub.ServicesPatterns.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace InventoryHub.Models
{
    public class Category: AuditableEntity
    {
        [Key]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Category Name is required")]
        [StringLength(100)]
        public string Name { get; set; }
        [Required(ErrorMessage ="Code is required")]
        public string Code { get; set; }
        [JsonIgnore]
        public ICollection<Product> Products { get; set; }
    }
}
