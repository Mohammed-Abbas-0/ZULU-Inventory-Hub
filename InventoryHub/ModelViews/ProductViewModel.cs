using InventoryHub.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace InventoryHub.ModelViews
{
    public enum TypeOfSearch { 
        Previous= 1,
        Next,
        ByCode
    }
    public class ProductViewModel
    {
        public int ProductId { get; set; }

        public string Name { get; set; }
        public string Code { get; set; } = "";

        public string Description { get; set; }

        public decimal Price { get; set; }

        public string ImagePath { get; set; }
        public int CategoryId { get; set; }
        public CategoryViewModel Category { get; set; } // نوع الكائن الذي يعبر عن Category
     
        public decimal? StockQuantity { get; set; }
        public int? StockId { get; set; }
        public StoreViewModel Store { get; set; } // نوع الكائن الذي يعبر عن Category
        public TypeOfSearch? SearchType { get; set; }
    }


}
