using InventoryHub.ServicesPatterns.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace InventoryHub.Models
{
    public class Product : AuditableEntity
    {
        [Key]
        public int ProductId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }
        [Required]
        public string Code { get; set; }
        [StringLength(500)]
        public string Description { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be a positive value.")]
        public decimal Price { get; set; }
        [Required]
        public decimal? StockQuantity { get; set; }
        public string ImagePath { get; set; }
        [ForeignKey("Category")]
        [Required]
        public int CategoryId { get; set; }
        [JsonIgnore]
        public Category Category { get; set; }
        //[Required]
        [ForeignKey("Store")]
        public int? StoreId { get; set; }
        [JsonIgnore]
        public Store Store { get; set; }
    }
    

}
