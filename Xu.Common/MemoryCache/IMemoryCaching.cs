namespace Xu.Common
{
    /// <summary>
    /// 简单的缓存接口，只有查询和添加，以后会进行扩展
    /// </summary>
    public interface IMemoryCaching
    {
        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <param name="cacheKey">缓存Key</param>
        /// <returns></returns>
        object Get(string cacheKey);

        /// <summary>
        /// 添加缓存
        /// </summary>
        /// <param name="cacheKey">缓存Key</param>
        /// <param name="cacheValue">缓存value</param>
        /// <param name="ExpirtionTime">滑动过期时间 XXX秒没有访问则清除</param>
        void Set(string cacheKey, object cacheValue, int ExpirtionTime = 7200);

        /// <summary>
        /// 验证缓存项是否存在
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <returns></returns>
        bool Exists(string cacheKey);

        /// <summary>
        /// 移除
        /// </summary>
        /// <param name="cacheKey"></param>
        /// <returns></returns>
        bool Remove(string cacheKey);
    }
}