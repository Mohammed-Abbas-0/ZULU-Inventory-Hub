using InventoryHub.Context;
using InventoryHub.Models;
using InventoryHub.ServicesPatterns.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryHub.ServicesPatterns.Implementation
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        public IRepository<Product> Products { get; private set; }
        public IRepository<Category> Categories { get; private set; }
        public IRepository<Store> Stores { get; private set; }

        public IRepository<Customer> Customers { get; private set; }

        public IRepository<Invoice> Invoices { get; private set; }

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
            Products = new Repository<Product>(_context);
            Categories = new Repository<Category>(_context);
            Stores = new Repository<Store>(_context);
            Customers = new Repository<Customer>(_context);
        }
        public async Task<int> CompleteAsync() => await _context.SaveChangesAsync();

        public void Dispose() => _context.Dispose();

    }

}
