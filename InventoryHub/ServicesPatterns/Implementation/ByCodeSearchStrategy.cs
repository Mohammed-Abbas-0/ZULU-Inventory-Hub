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
    public class ByCodeSearchStrategy : ISearchStrategy
    {
        private readonly AppDbContext _context;
        public ByCodeSearchStrategy(AppDbContext context) => _context = context;

        public async Task<Product> Search(int id, string code)
        {
            var PreviousProduct = await _context.Products
                .Where(idx => idx.Code == code)
                .Include(idx => idx.Category)
                .Include(idx => idx.Store)
                .FirstOrDefaultAsync();
            if (PreviousProduct != null)
                return PreviousProduct;
            return null;
        }
    }
}
