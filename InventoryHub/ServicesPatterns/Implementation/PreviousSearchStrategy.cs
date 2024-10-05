﻿using InventoryHub.Context;
using InventoryHub.Models;
using InventoryHub.ServicesPatterns.IService;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryHub.ServicesPatterns.Implementation
{
    public class PreviousSearchStrategy : ISearchStrategy
    {
        private readonly AppDbContext _context;
        public PreviousSearchStrategy(AppDbContext context) => _context = context;
        
        public async Task<Product> Search(int id, string code)
        {
            var PreviousProduct = await _context.Products
                .OrderByDescending(idx=>idx.ProductId)
                .Where(idx => idx.ProductId < id)
                .Include(idx=>idx.Category)
                .Include(idx=>idx.Store)
                .FirstOrDefaultAsync();
            if (PreviousProduct != null)
                return PreviousProduct;
            Product firstProduct = await _context.Products
                .Include(idx => idx.Category)
                .Include(idx => idx.Store)
                .FirstOrDefaultAsync();
            if (firstProduct != null)
                return firstProduct;
            return null;
        }
    }
}