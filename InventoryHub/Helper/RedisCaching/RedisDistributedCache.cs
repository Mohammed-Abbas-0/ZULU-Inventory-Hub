using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryHub.Helper.RedisCaching
{
    public class RedisDistributedCache<T> : IRedisDistributedCache<T> where T:class
    {
        private readonly IDistributedCache _distributedCache;
        public RedisDistributedCache(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
        }
        public async Task<T> GetEntryAsync(string key)
        {
            var data = await _distributedCache.GetStringAsync(key);
            if (string.IsNullOrEmpty(data))
                return null;

            T getObject = JsonConvert.DeserializeObject<T>(data);
            return getObject;
        }

        

        public async Task SetEntryAsync(string key, T data, TimeSpan? absoluteExpireTime = null, TimeSpan? slidingExpireTime = null)
        {
            var options = new DistributedCacheEntryOptions();
            options.AbsoluteExpirationRelativeToNow = absoluteExpireTime ?? TimeSpan.FromMinutes(1);
            options.SlidingExpiration = slidingExpireTime;
            var jsonData = JsonConvert.SerializeObject(data);
            await _distributedCache.SetStringAsync(key,jsonData,options);
        }
    }
}
