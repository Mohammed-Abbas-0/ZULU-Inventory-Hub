using InventoryHub.Context;
using InventoryHub.Models;
using InventoryHub.ModelViews;
using InventoryHub.ServicesPatterns.IService;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryHub.ServicesPatterns.Implementation
{
    public class CategoryQueryService: ICategoryQueryService
    {
        private readonly AppDbContext _context;
        public CategoryQueryService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<CategoryViewModel>> GetCategoriesAsync(int pageNumber , int pageSize )
        {


            var categories = 
                await ( from category in  _context.Categories.Skip((pageNumber - 1) * pageSize).Take(pageSize)
                        join product in _context.Products on category.CategoryId equals product.CategoryId into categoryProducts
                        from product in categoryProducts.DefaultIfEmpty() 
                        select new CategoryViewModel
                        {
                            Name = category.Name,
                            Code = category.Code,
                            CategoryId = category.CategoryId,
                            TotalProducts = product.ProductId != null?1:0
                        }
                    )
                .ToListAsync();

            var groupCategory = categories
                .GroupBy(idx=>idx.CategoryId)
                .Select(idx=> new CategoryViewModel
                {
                    CategoryId = idx.Key,
                    Code = idx.FirstOrDefault().Code,
                    Name = idx.FirstOrDefault().Name,
                    TotalProducts = idx.Select(i => i.TotalProducts).Sum(),
                })
                .ToList();

            return groupCategory;
        }

        public async Task<int> GetCountCategoriesAsync()
            => await _context.Categories.AsNoTracking().CountAsync();

        public async Task<Category> GetCategoryByCodeAsync(string Code)
            => await _context.Categories.AsNoTracking().Where(idx => idx.Code == Code).FirstOrDefaultAsync();
    }
}
