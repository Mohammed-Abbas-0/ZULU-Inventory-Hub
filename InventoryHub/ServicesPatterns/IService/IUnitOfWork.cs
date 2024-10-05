using InventoryHub.Models;
using InventoryHub.ServicesPatterns.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryHub.ServicesPatterns.IService
{
    public interface IUnitOfWork:IDisposable
    {
        IRepository<Product> Products { get; }
        IRepository<Category> Categories { get; }
        IRepository<Store> Stores { get; }
        IRepository<Customer> Customers { get; }
        IRepository<Invoice> Invoices { get; }
        Task<int> CompleteAsync();
    }
}
