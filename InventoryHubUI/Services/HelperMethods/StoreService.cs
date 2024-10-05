using InventoryHubUI.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace InventoryHubUI.Services.HelperMethods
{
    public class StoreService : IStoreService
    {
        private readonly HttpClient _httpClient;

        public StoreService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<Store>> GetStoresAsync()
        {
            // استدعاء API التصنيفات
            var response = await _httpClient.GetAsync(API_EndPoint.StoreENDPoint);
            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                var stocks = JsonConvert.DeserializeObject<List<Store>>(jsonString);
                return stocks;
            }
            return new List<Store>();
        }
    }
}
