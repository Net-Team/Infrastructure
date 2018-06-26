using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System
{
    /// <summary>
    /// 可空类型扩展
    /// </summary>
    public static partial class NullableExtend
    {
        /// <summary>
        /// 如果为空则返回类型的初始值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static T NullThenDefault<T>(this Nullable<T> source) where T : struct
        {
            if (source.HasValue)
            {
                return source.Value;
            }
            return default(T);
        }

        /// <summary>
        /// 如果为空则返回类型的初始值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static T NullThen<T>(this Nullable<T> source, T value) where T : struct
        {
            if (source.HasValue)
            {
                return source.Value;
            }
            return value;
        }


        /// <summary>
        /// 转换为分页索引
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static int ToIndex(this Nullable<int> source)
        {
            if (source.HasValue == false || source.Value <= 0)
            {
                return 0;
            }
            return source.Value - 1;
        }
    }
}
