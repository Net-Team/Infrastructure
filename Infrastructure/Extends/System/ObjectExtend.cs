using Newtonsoft.Json;
using System.Text.RegularExpressions;

namespace System
{
    /// <summary>
    /// Object扩展
    /// </summary>
    public static partial class ObjectExtend
    {
        /// <summary>
        /// 强制转换为目标类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static T OfType<T>(this object source)
        {
            return (T)source;
        }

        /// <summary>
        /// 转换为友好的调试字符串
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string ToDebugString(this object source)
        {
            if (source == null)
            {
                return null;
            }

            var json = JsonConvert.SerializeObject(source, Formatting.Indented);
            return Regex.Replace(json, "\"\\w+\"(?=:)", (m) => m.Value.Substring(1, m.Value.Length - 2));
        }
    }
}
