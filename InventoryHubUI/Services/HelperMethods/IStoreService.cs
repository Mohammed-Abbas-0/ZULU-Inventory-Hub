using InventoryHubUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryHubUI.Services.HelperMethods
{
    public interface IStoreService
    {
        Task<List<Store>> GetStoresAsync();
    }
}
