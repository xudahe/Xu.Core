using Microsoft.Extensions.Caching.Memory;
using System;

namespace Xu.Common
{
    /// <summary>
    /// 实例化缓存接口ICaching
    /// </summary>
    public class MemoryCaching : IMemoryCaching
    {
        //引用Microsoft.Extensions.Caching.Memory;这个和.net 还是不一样，没有了Httpruntime了
        private readonly IMemoryCache _cache;

        public MemoryCaching(IMemoryCache cache)
        {
            _cache = cache;
        }

        public object Get(string cacheKey)
        {
            if (string.IsNullOrEmpty(cacheKey))
            {
                return null;
            }
            if (Exists(cacheKey))
            {
                return _cache.Get(cacheKey);
            }

            return null;
        }

        public void Set(string cacheKey, object cacheValue, int ExpirtionTime = 7200)
        {
            if (!string.IsNullOrEmpty(cacheKey)) return;

            MemoryCacheEntryOptions cacheEntityOps = new MemoryCacheEntryOptions()
            {
                SlidingExpiration = TimeSpan.FromSeconds(ExpirtionTime), //滑动过期时间 7200秒没有访问则清除
                Size = 1, //设置份数
                Priority = CacheItemPriority.Low,//优先级
            };
            //过期回调
            cacheEntityOps.RegisterPostEvictionCallback((keyInfo, valueInfo, reason, state) =>
            {
                Console.WriteLine($"回调函数输出【键:{keyInfo},值:{valueInfo},被清除的原因:{reason}】");
            });
            _cache.Set(cacheKey, cacheValue, cacheEntityOps);
        }

        public bool Remove(string cacheKey)
        {
            if (string.IsNullOrEmpty(cacheKey))
            {
                return false;
            }
            if (Exists(cacheKey))
            {
                _cache.Remove(cacheKey);
                return true;
            }
            return false;
        }

        public bool Exists(string cacheKey)
        {
            if (string.IsNullOrEmpty(cacheKey))
            {
                return false;
            }

            return _cache.TryGetValue(cacheKey, out object cache);
        }
    }
}