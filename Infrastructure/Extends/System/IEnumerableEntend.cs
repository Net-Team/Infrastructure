using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace System
{
    /// <summary>
    /// 可迭代类扩展
    /// </summary>
    public static partial class IEnumerableEntend
    {
        /// <summary>
        /// 表示空的分组对象
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="T"></typeparam>
        private class EmptyGroup<TKey, T> : IGrouping<TKey, T>
        {
            public TKey Key { get; private set; }

            public EmptyGroup(TKey key)
            {
                this.Key = key;
            }

            private IEnumerable<T> GetItems()
            {
                yield break;
            }

            public IEnumerator<T> GetEnumerator()
            {
                return this.GetItems().GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.GetEnumerator();
            }
        }

        /// <summary>
        /// 为Null则返回0条记录的迭代
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static IEnumerable<T> NullThenEmpty<T>(this IEnumerable<T> source)
        {
            if (source != null)
            {
                return source;
            }
            return Enumerable.Empty<T>();
        }

        /// <summary>
        /// 以keys为基准，补齐分组集合
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <typeparam name="TKey">键类型</typeparam>
        /// <param name="source"></param>
        /// <param name="keys">所有分组的key集合</param>
        /// <returns></returns>
        public static IEnumerable<IGrouping<TKey, T>> PadAs<T, TKey>(this IEnumerable<IGrouping<TKey, T>> source, IEnumerable<TKey> keys)
        {
            return source.PadAs(item => item.Key, keys, key => new EmptyGroup<TKey, T>(key));
        }

        /// <summary>
        /// 以keys为基准，补齐集合的内容
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <typeparam name="TKey">键类型</typeparam>
        /// <param name="source"></param>
        /// <param name="keySelector">键选择</param>
        /// <param name="keys">基准keys</param>
        /// <returns></returns>
        public static IEnumerable<T> PadAs<T, TKey>(this IEnumerable<T> source, Func<T, TKey> keySelector, IEnumerable<TKey> keys)
        {
            return source.PadAs(keySelector, keys, key => default(T));
        }

        /// <summary>
        /// 以keys为基准，补齐集合的内容
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <typeparam name="TKey">键类型</typeparam>
        /// <param name="source"></param>
        /// <param name="keySelector">键选择</param>
        /// <param name="keys">基准keys</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns></returns>
        public static IEnumerable<T> PadAs<T, TKey>(this IEnumerable<T> source, Func<T, TKey> keySelector, IEnumerable<TKey> keys, Func<TKey, T> defaultValue)
        {
            var q = from l in keys.NullThenEmpty()
                    join r in source.NullThenEmpty()
                    on l equals keySelector(r)
                    into g
                    from item in g.DefaultIfEmpty()
                    select item == null ? defaultValue(l) : item;
            return q;
        }
    }
}
