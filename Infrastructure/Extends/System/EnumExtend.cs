using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace System
{
    /// <summary>
    /// 枚举扩展
    /// </summary>
    public static partial class EnumExtend
    {
        /// <summary>
        /// 获取枚举类型所有字段的名称
        /// </summary>
        /// <param name="e">枚举类型</param>
        /// <returns></returns>
        public static string[] GetFieldNames(this Enum e)
        {
            return Enum.GetNames(e.GetType());
        }


        /// <summary>
        /// 获取枚举类型所有字段的值
        /// </summary>
        /// <typeparam name="T">值类型</typeparam>
        /// <param name="e">枚举类型</param>
        /// <returns></returns>
        public static T[] GetFieldValues<T>(this Enum e)
        {
            return Enum.GetValues(e.GetType()).Cast<T>().ToArray();
        }

        /// <summary>
        /// 获取枚举类型所有字段的值
        /// </summary>
        /// <param name="e">枚举类型</param>
        /// <returns></returns>
        public static Enum[] GetFieldValues(this Enum e)
        {
            return e.GetFieldValues<Enum>();
        }

        /// <summary>
        /// 获取特性
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="e"></param>
        /// <returns></returns>
        public static T GetAttribute<T>(this Enum e) where T : class
        {
            var field = e.GetType().GetField(e.ToString());
            var attribute = Attribute.GetCustomAttribute(field, typeof(T)) as T;
            return attribute;
        }

        /// <summary>
        /// 获取特性
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="e"></param>
        /// <returns></returns>
        public static T[] GetAttributes<T>(this Enum e) where T : class
        {
            var field = e.GetType().GetField(e.ToString());
            var attributes = Attribute.GetCustomAttributes(field, typeof(T)) as T[];
            return attributes;
        }

        /// <summary>
        /// 获取枚举字段的Display特性的名称
        /// </summary>
        /// <param name="e">枚举字段</param>
        /// <returns></returns>
        public static string GetFieldDisplay(this Enum e)
        {
            if (e == null)
            {
                return null;
            }

            var display = e.GetAttribute<DisplayAttribute>();
            if (display == null)
            {
                return e.ToString();
            }
            return display.Name;
        }

        /// <summary>
        /// 根据枚举字段的Display特性的说明
        /// </summary>
        /// <param name="e">枚举字段</param>
        /// <returns></returns>
        public static string GetFieldDescription(this Enum e)
        {
            if (e == null)
            {
                return null;
            }
            var display = e.GetAttribute<DisplayAttribute>();
            return display == null ? null : display.Description;
        }

        /// <summary>
        /// 获取枚举类型所有字段对应的Dispaly特性名称和说明
        /// </summary>
        /// <param name="e">枚举类型</param>
        /// <returns></returns>
        public static IEnumerable<KeyValuePair<string, string>> GetDisplays(this Enum e)
        {
            return e.GetFieldValues().Select(item => item.GetAttribute<DisplayAttribute>()).Select(item => new KeyValuePair<string, string>(item.Name, item.Description));
        }

        /// <summary>
        /// 获取枚举类型所有字段名称和对应的Dispaly特性名称
        /// </summary>
        /// <param name="e">枚举类型</param>
        /// <returns></returns>
        public static IEnumerable<KeyValuePair<string, string>> GetNameDisplays(this Enum e)
        {
            return e.GetFieldValues().Select(item => new KeyValuePair<string, string>(item.ToString(), item.GetFieldDisplay()));
        }

        /// <summary>
        /// 获取枚举类型所有字段值和对应的Dispaly特性名称
        /// </summary>
        /// <param name="e">枚举类型</param>
        /// <returns></returns>
        public static IEnumerable<KeyValuePair<Enum, string>> GetValueDisplays(this Enum e)
        {
            return e.GetFieldValues().Select(item => new KeyValuePair<Enum, string>(item, item.GetFieldDisplay()));
        }

        /// <summary>
        /// 是否包含目标值
        /// </summary>
        /// <param name="e"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsFlagContains(this Enum e, Enum value)
        {
            return 0 != (value.GetHashCode() & e.GetHashCode());
        }

        /// <summary>
        /// 获取枚举值包含的位域值
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public static IEnumerable<Enum> GetFlagEnums(this Enum e)
        {
            if (e.GetHashCode() == 0)
            {
                return new Enum[0];
            }
            return e.GetFieldValues().Where(item => e.IsFlagContains(item));
        }

        /// <summary>
        /// 获取枚举值包含的位域值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="e"></param>
        /// <returns></returns>
        public static IEnumerable<T> GetFlagEnums<T>(this Enum e) where T : struct
        {
            return e.GetFlagEnums().Cast<T>();
        }

        /// <summary>
        /// 是否声明特性
        /// </summary>
        /// <typeparam name="T">特性类型</typeparam>
        /// <param name="e">枚举</param>
        /// <returns></returns>
        public static bool IsDefined<T>(this Enum e) where T : class
        {
            var field = e.GetType().GetField(e.ToString());
            return Attribute.IsDefined(field, typeof(T));
        }
    }
}
