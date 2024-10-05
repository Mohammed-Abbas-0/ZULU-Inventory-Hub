using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryHub.Helper.RedisCaching
{
    public interface IRedisDistributedCache<T>
    {
        Task SetEntryAsync(string key,T data,TimeSpan? absoluteExpireTime = null,TimeSpan? slidingExpireTime = null);
        Task<T> GetEntryAsync(string key);
    }
}
