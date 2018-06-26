using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using System.Reflection;
using System.Linq.Expressions;
using Infrastructure.Reflection;

namespace Infrastructure.Utility
{
    /// <summary>
    /// 提供同类型对象实例的条件更新
    /// </summary>
    public static class Updater
    {
        /// <summary>
        /// 更新实例
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">原实例</param>
        /// <param name="target">目标实例</param>
        /// <param name="fieldUpdate">字段更新条件</param>
        /// <returns></returns>
        public static T Update<T>(T source, T target, Func<string, bool> fieldUpdate)
        {
            return Updater.Update<T, object>(source, target, fieldUpdate, null);
        }

        /// <summary>
        /// 更新实例
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">原实例</param>
        /// <param name="target">目标实例</param>
        /// <param name="fieldUpdate">字段更新条件</param>
        /// <param name="except">排除更新的字段</param>
        /// <returns></returns>
        public static T Update<T, TExcept>(T source, T target, Func<string, bool> fieldUpdate, Expression<Func<T, TExcept>> except)
        {
            var properties = Property.GetProperties(typeof(T));
            var exceptMembers = except == null ? Enumerable.Empty<MemberInfo>() : (except.Body as NewExpression).Members;

            foreach (var item in properties)
            {
                if (fieldUpdate(item.Name) && exceptMembers.Any(m => m.Name == item.Name) == false)
                {
                    var value = item.GetValue(source);
                    item.SetValue(target, value);
                }
            }
            return target;
        }

        /// <summary>
        /// 将source映射到target
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TTarget"></typeparam>
        /// <param name="source">数据源</param>
        /// <param name="target">目标</param>       
        /// <returns></returns>
        public static TTarget Map<TSource, TTarget>(TSource source) where TTarget : new()
        {
            return Updater.Map<TSource, TTarget>(source, new TTarget());
        }

        /// <summary>
        /// 将source映射到target
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TTarget"></typeparam>
        /// <param name="source">数据源</param>
        /// <param name="target">目标</param>       
        /// <returns></returns>
        public static TTarget Map<TSource, TTarget>(TSource source, TTarget target)
        {
            var proertiesSource = Property.GetProperties(typeof(TSource));
            var proertiesTarget = Property.GetProperties(typeof(TTarget));

            foreach (var pSource in proertiesSource)
            {
                var pTarget = proertiesTarget.FirstOrDefault(item => item.Name == pSource.Name);
                if (pTarget == null)
                {
                    continue;
                }
                var value = pSource.GetValue(source);
                var valueCast = Converter.Cast(value, pTarget.Info.PropertyType);
                pTarget.SetValue(target, valueCast);
            }
            return target;
        }
    }
}
