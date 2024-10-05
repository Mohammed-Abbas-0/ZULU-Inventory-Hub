using InventoryHubUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryHubUI.DTO
{
    public class ProductViewModel
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public string ImagePath { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; }
        public decimal? StockQuantity { get; set; }
        public int? StockId { get; set; }
        public Store Store { get; set; } // نوع الكائن الذي يعبر عن Category

    }
}
