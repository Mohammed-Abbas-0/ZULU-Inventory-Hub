using InventoryHub.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryHub.ServicesPatterns.IService
{
    public interface ICustomerQueryService
    {
        Task<IEnumerable<Customer>> GeCutomersAsync(int pageNumber, int pageSize);
        Task<bool> ChangeStatus(int id);
    }
}
