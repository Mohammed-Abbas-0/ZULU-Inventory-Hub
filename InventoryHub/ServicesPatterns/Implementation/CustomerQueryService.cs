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
    public class CutomerQueryService : ICustomerQueryService
    {
        private readonly AppDbContext _context;
        public CutomerQueryService(AppDbContext context)
        {
            _context = context;
        }
         
        public async Task<bool> ChangeStatus(int id)
        {
            var Customer = await _context.Customers.FindAsync(id);
            if (Customer is null)
                return false;
            Customer.IsActive = (Customer.IsActive) ? false : true;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Customer>> GeCutomersAsync(int pageNumber, int pageSize)
        {
            var products = await _context.Customers
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
            return products;
        }

        public async Task<bool> IsExisted(Customer customer)
        {
            var isExisted = await _context.Customers
                .AnyAsync(idx =>
                (idx.Email.Trim() == customer.Email.Trim()) 
                    || (idx.FirstName + idx.LastName == customer.FirstName + customer.LastName)
                );
            return isExisted;
        }
    }
}
