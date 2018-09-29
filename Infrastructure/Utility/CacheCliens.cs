using System;

namespace Infrastructure.Utility
{
    /// <summary>
    /// 自定义缓存数据
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CacheCliens<T> : MemoryCaches<T>
    {
        /// <summary>
        /// 构造自定义缓存,默认缓存1小时
        /// </summary>
        /// <param name="expireTime">过期时间</param>
        public CacheCliens() :
            base(typeof(T).Name)
        {
        }

        /// <summary>
        /// 构造自定义缓存
        /// </summary>
        /// <param name="expireTime">过期时间</param>
        public CacheCliens(TimeSpan expireTime) :
            base(typeof(T).Name, expireTime)
        {
        }
    }
}
