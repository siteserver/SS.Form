using System;
using System.Web;
using System.Web.Caching;

namespace SS.Form.Core.Utils
{
    public static class CacheUtils
    {
        private static readonly Cache Cache;

        static CacheUtils()
        {
            var context = HttpContext.Current;
            Cache = context != null ? context.Cache : HttpRuntime.Cache;
        }

        public static void Remove(string key)
        {
            Cache.Remove(key);
        }

        public static void InsertMinutes(string key, object obj, int minutes)
        {
            InnerInsert(key, obj, null, TimeSpan.FromMinutes(minutes));
        }

        public static void InsertHours(string key, object obj, int hours)
        {
            InnerInsert(key, obj, null, TimeSpan.FromHours(hours));
        }

        private static void InnerInsert(string key, object obj, string filePath, TimeSpan timeSpan)
        {
            if (!string.IsNullOrEmpty(key) && obj != null)
            {
                Cache.Insert(key, obj, string.IsNullOrEmpty(filePath) ? null : new CacheDependency(filePath), Cache.NoAbsoluteExpiration, timeSpan, CacheItemPriority.Normal, null);
            }
        }

        public static T Get<T>(string key) where T : class
        {
            return Cache.Get(key) as T;
        }
        
    }
}
