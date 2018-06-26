using Infrastructure.Reflection;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Utility
{
    /// <summary>
    /// 提供相同名称字段的对象映射
    /// </summary>
    public class Mapper
    {
        /// <summary>
        /// 数据源
        /// </summary>
        private readonly object source;

        /// <summary>
        /// 相同名称字段的对象映射
        /// </summary>
        /// <param name="source">数据源</param>
        public Mapper(object source)
        {
            this.source = source;
        }

        /// <summary>
        /// 映射为目标类型
        /// </summary>
        /// <typeparam name="TTarget">目标类型</typeparam>
        /// <returns></returns>
        public TTarget To<TTarget>() where TTarget : new()
        {
            if (this.source == null)
            {
                return default(TTarget);
            }

            return this.To<TTarget>(new TTarget());
        }

        /// <summary>
        /// 更新到目标对象实例
        /// </summary>
        /// <typeparam name="TTarget">目标类型</typeparam>
        /// <param name="target">目标实例</param>
        /// <returns></returns>
        public TTarget To<TTarget>(TTarget target)
        {
            if (this.source == null)
            {
                return default(TTarget);
            }

            var proertiesSource = Mapper.GetProperties(this.source.GetType());
            var proertiesTarget = Mapper.GetProperties(typeof(TTarget));

            foreach (var pSource in proertiesSource)
            {
                Property pTarget;
                if (proertiesTarget.TryGetValue(pSource.Key, out pTarget) == false)
                {
                    continue;
                }
                var value = pSource.Value.GetValue(this.source);
                var valueCast = Converter.Cast(value, pTarget.Info.PropertyType);
                pTarget.SetValue(target, valueCast);
            }
            return target;
        }

        /// <summary>
        /// 转换为字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.source.ToString();
        }

        /// <summary>
        /// 类型属性的Setter缓存
        /// </summary>
        private static readonly ConcurrentDictionary<Type, IDictionary<string, Property>> cached = new ConcurrentDictionary<Type, IDictionary<string, Property>>();


        /// <summary>
        /// 从类型的属性获取属性
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns></returns>
        private static IDictionary<string, Property> GetProperties(Type type)
        {
            return cached.GetOrAdd(type, t =>
              t.GetProperties()
              .Select(p => new Property(p))
              .ToDictionary(p => p.Name, p => p, StringComparer.OrdinalIgnoreCase)
           );
        }
    }
}
