using InventoryHubUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryHubUI.Services.HelperMethods
{
    public interface ICategoryService
    {
        Task<List<Category>> GetCategoriesAsync();
    }
}
