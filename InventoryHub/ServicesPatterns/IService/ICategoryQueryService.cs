using InventoryHub.Models;
using InventoryHub.ModelViews;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryHub.ServicesPatterns.IService
{
    public interface ICategoryQueryService
    {
        Task<List<CategoryViewModel>> GetCategoriesAsync(int pageNumber, int pageSize);
        Task<int> GetCountCategoriesAsync();
        Task<Category> GetCategoryByCodeAsync(string Code);
    }
}
