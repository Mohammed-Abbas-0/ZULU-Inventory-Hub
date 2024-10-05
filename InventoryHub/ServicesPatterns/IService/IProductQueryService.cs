using InventoryHub.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryHub.ServicesPatterns.IService
{
    public interface IProductQueryService
    {
        Task<IEnumerable<Product>> GetProductsWithCategoryAndStockAsync(int pageNumber, int pageSize);
        Task<bool> ChangeProductImage(int productId, string image);
        Task<int> GetTotalProductsCountAsync();
    }
}
