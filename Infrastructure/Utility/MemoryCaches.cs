using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;

namespace Infrastructure.Utility
{
    /// <summary>
    /// 缓存数据
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MemoryCaches<T>
    {
        /// <summary>
        /// 缓存前缀
        /// </summary>
        private static readonly string _prefix = "Cache_";

        /// <summary>
        /// 同步锁
        /// </summary>
        public readonly object SyncRoot = new object();

        /// <summary>
        /// 创建缓存对象
        /// </summary>
        private MemoryCache memoryCaches { get; set; }

        /// <summary>
        /// 缓存过期时间设置,默认1分钟
        /// </summary>
        private TimeSpan timeSpan { get; set; } = TimeSpan.FromMinutes(1);

        /// <summary>
        /// 构造函数，默认过期时间 1分钟
        /// </summary>
        /// <param name="Tkey"></param>
        public MemoryCaches(string Tkey)
        {
            var keyValue = MergeKey(Tkey);
            this.memoryCaches = new MemoryCache(keyValue);
        }

        /// <summary>
        /// 构造函数，自定义默认过期时间
        /// </summary>
        /// <param name="Tkey"></param>
        /// <param name="timeSpan"></param>
        public MemoryCaches(string Tkey, TimeSpan timeSpan)
        {
            var keyValue = MergeKey(Tkey);
            this.memoryCaches = new MemoryCache(keyValue);
            this.timeSpan = timeSpan;
        }

        /// <summary>
        /// 添加缓存
        /// </summary>
        /// <param name="key">缓存唯一Key</param>
        /// <param name="data">缓存内容</param>
        /// <param name="timeSpan"></param>
        public virtual void AddOrUpdate(string key, T data)
        {
            lock (this.SyncRoot)
            {
                if (this.memoryCaches.Any(item => item.Key == key))
                {
                    this.Update(key, data);
                }
                else
                {
                    this.memoryCaches.Add(key, data, DateTimeOffset.Now.Add(timeSpan));
                }
            }
        }

        /// <summary>
        /// 更新缓存数据
        /// </summary>
        /// <param name="key">缓存唯一Key</param>
        /// <param name="data">缓存内容</param>
        private void Update(string key, T data)
        {
            lock (this.SyncRoot)
            {
                this.memoryCaches.Set(key, data, DateTimeOffset.Now.Add(timeSpan));
            }
        }

        /// <summary>
        /// 删除缓存数据
        /// </summary>
        /// <param name="key">缓存唯一Key</param>
        public virtual void Delete(string key)
        {
            lock (this.SyncRoot)
            {
                this.memoryCaches.Remove(key);
            }
        }

        /// <summary>
        /// 获取所有缓存数据
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<T> Where(Func<KeyValuePair<string, object>, bool> where)
        {
            lock (this.SyncRoot)
            {
                var values = this.memoryCaches.Where(where).ToList();
                foreach (var item in values)
                {
                    yield return (T)item.Value;
                }
            }
        }

        /// <summary>
        /// 获取缓存数据
        /// </summary>
        /// <param name="key">缓存唯一Key</param>
        /// <returns></returns>
        public virtual T Get(string key)
        {
            lock (this.SyncRoot)
            {
                var obj = this.memoryCaches.Get(key);
                if (obj == null)
                {
                    return default(T);
                }
                else
                {
                    return (T)obj;
                }
            }
        }

        /// <summary>
        /// 获取数据库名
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private static string MergeKey(string name)
        {
            return $"{_prefix}{name}";
        }
    }
}
