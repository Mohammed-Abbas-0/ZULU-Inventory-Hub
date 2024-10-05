using InventoryHub.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryHub.ServicesPatterns.IService
{
    public interface ISearchStrategy
    {
        Task<Product> Search(int id, string code);
    }
}
