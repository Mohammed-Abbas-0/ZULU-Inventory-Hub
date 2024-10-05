using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryHub.ModelViews
{
    public class ProductImage
    {
        public int ProductId { get; set; }
        public IFormFile ImagePath { get; set; }
    }

}
