using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace InventoryHubUI.Models
{
    public class Product
    {
        [Key]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Product Name is required.")]
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(500)]
        public string Description { get; set; }
        public string Code { get; set; }
       
        public decimal? StockQuantity { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be a positive value.")]
        public decimal Price { get; set; }
        public string ImagePath { get; set; }
        [Required]
        public int CategoryId { get; set; }
        [JsonIgnore]
        public Category Category { get; set; }
      
        public int? StoreId { get; set; }
        [JsonIgnore]
        public Store Store { get; set; }
    }
    

}
