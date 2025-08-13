using Microsoft.Extensions.Caching.Memory;
using System;

namespace ZestPost.Service
{
    public class CachingService
    {
        private static readonly MemoryCache Cache = new MemoryCache(new MemoryCacheOptions());
        private static readonly TimeSpan DefaultExpiration = TimeSpan.FromMinutes(30);

        public T Get<T>(string key)
        {
            return Cache.Get<T>(key);
        }

        public void Set<T>(string key, T value, TimeSpan? expiration = null)
        {
            var cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiration ?? DefaultExpiration
            };
            Cache.Set(key, value, cacheEntryOptions);
        }

        public void Remove(string key)
        {
            Cache.Remove(key);
        }
    }
}
