using InventoryHub.Context;
using InventoryHub.Models;
using InventoryHub.ServicesPatterns.IService;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryHub.ServicesPatterns.Implementation
{
    public class ProductQueryService : IProductQueryService
    {
        private readonly AppDbContext _context;
        public ProductQueryService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> ChangeProductImage(int productId, string image)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product is null)
                return false;
            product.ImagePath = image;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Product>> GetProductsWithCategoryAndStockAsync(int pageNumber, int pageSize)
        {
            var products = await _context.Products
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .Include(p => p.Category)
                    .Include(p => p.Store)
                    .ToListAsync();
            return products;
        }
        public async Task<int> GetTotalProductsCountAsync() => await _context.Products.AsNoTracking().CountAsync();
    }
}
