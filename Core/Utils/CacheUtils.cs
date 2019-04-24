using System;
using System.Runtime.Caching;

namespace SS.Form.Core.Utils
{
    public static class CacheUtils
    {
        public static void Remove(string key)
        {
            ObjectCache cache = MemoryCache.Default;
            cache.Remove(key);
        }

        public static void Insert(string key, object obj, int hours)
        {
            if (!string.IsNullOrEmpty(key) && obj != null)
            {
                ObjectCache cache = MemoryCache.Default;
                var policy = new CacheItemPolicy
                {
                    SlidingExpiration = new TimeSpan(0, hours, 0, 0)
                };
                cache.Set(key, obj, policy);
            }
        }

        public static T Get<T>(string key) where T : class
        {
            ObjectCache cache = MemoryCache.Default;
            return cache[key] as T;
        }
    }
}
