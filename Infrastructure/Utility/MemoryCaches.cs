using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Threading.Tasks;

namespace Infrastructure.Utility
{
    /// <summary>
    /// 缓存数据
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MemoryCaches<T>
    {
        /// <summary>
        /// 异步锁
        /// </summary>
        private readonly AsyncRoot asyncRoot = new AsyncRoot();

        /// <summary>
        /// 创建缓存对象
        /// </summary>
        private readonly MemoryCache memoryCaches;

        /// <summary>
        /// 缓存过期时间设置,默认1分钟
        /// </summary>
        private TimeSpan ExpireTime { get; set; }

        /// <summary>
        /// 获取过期时间
        /// </summary>
        /// <returns></returns>
        private DateTimeOffset GetDateTimeOffset()
        {
            return DateTimeOffset.Now.Add(this.ExpireTime);
        }


        /// <summary>
        /// 构造随机缓存名，默认过期时间 5分钟
        /// </summary>
        public MemoryCaches()
           : this(Guid.NewGuid().ToString(), TimeSpan.FromMinutes(5))
        {
        }

        /// <summary>
        /// 构造随机缓存名，指定过期时间
        /// </summary>
        public MemoryCaches(TimeSpan expireTime)
           : this(Guid.NewGuid().ToString(), expireTime)
        {
        }

        /// <summary>
        /// 构造自定义缓存名,默认过期时间 5分钟
        /// </summary>
        /// <param name="name"></param>
        public MemoryCaches(string name)
            : this(name, TimeSpan.FromMinutes(5))
        {
        }

        /// <summary>
        /// 构造自定义缓存名,指定过期时间
        /// </summary>
        /// <param name="name"></param>
        public MemoryCaches(string name, TimeSpan expireTime)
        {
            this.ExpireTime = expireTime;
            this.memoryCaches = new MemoryCache(name);
        }


        /// <summary>
        /// 同步方式获取或添加缓存
        /// </summary>
        /// <param name="key">缓存唯一Key</param>
        /// <param name="data">缓存内容</param>
        /// <param name="timeSpan"></param>
        public T GetOrAdd(string key, Func<T> factory)
        {
            using (asyncRoot.Lock())
            {
                var value = this.memoryCaches.Get(key) as CacheValue;
                if (value == null)
                {

                    value = new CacheValue { Value = factory() };
                    this.memoryCaches.Add(key, value, this.GetDateTimeOffset());
                }
                return value.Value;
            }
        }

        /// <summary>
        /// 异步方式获取或添加缓存
        /// </summary>
        /// <param name="key">缓存唯一Key</param>
        /// <param name="data">缓存内容</param>
        /// <param name="timeSpan"></param>
        public async Task<T> GetOrAddAsync(string key, Func<Task<T>> factory)
        {
            using (await asyncRoot.LockAsync())
            {
                var value = this.memoryCaches.Get(key) as CacheValue;
                if (value == null)
                {
                    var cache = await factory();
                    value = new CacheValue { Value = cache };
                    this.memoryCaches.Add(key, value, this.GetDateTimeOffset());
                }
                return value.Value;
            }
        }


        /// <summary>
        /// 同步方式获取或添加缓存
        /// </summary>
        /// <param name="key">缓存唯一Key</param>
        /// <param name="data">缓存内容</param>
        /// <param name="timeSpan"></param>
        public void AddOrUpdate(string key, T data)
        {
            using (asyncRoot.Lock())
            {
                var value = new CacheValue { Value = data };
                this.memoryCaches.Set(key, value, this.GetDateTimeOffset());
            }
        }

        /// <summary>
        /// 根据Key 获取对象
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public T Get(string key)
        {
            using (asyncRoot.Lock())
            {
                var value = this.memoryCaches.Get(key) as CacheValue;
                if (value == null)
                {
                    return default(T);
                }
                return value.Value;
            }
        }

        /// <summary>
        /// 异步方式获取或添加缓存
        /// </summary>
        /// <param name="key">缓存唯一Key</param>
        /// <param name="data">缓存内容</param>
        /// <param name="timeSpan"></param>
        public async Task<T> AddOrUpdateAsync(string key, Func<Task<T>> factory)
        {
            using (await asyncRoot.LockAsync())
            {
                var value = this.memoryCaches.Get(key) as CacheValue;
                if (value == null)
                {
                    var cache = await factory();
                    value = new CacheValue { Value = cache };
                    this.memoryCaches.Add(key, value, this.GetDateTimeOffset());
                }
                return value.Value;
            }
        }

        /// <summary>
        /// 根据Key删除指定缓存
        /// </summary>
        /// <param name="key">缓存唯一Key</param>
        public void Delete(string key)
        {
            this.memoryCaches.Remove(key);
        }

        /// <summary>
        /// 缓存值
        /// </summary>
        class CacheValue
        {
            public T Value { get; set; }
        }
    }
}
