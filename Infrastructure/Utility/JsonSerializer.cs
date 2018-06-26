using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;

namespace Infrastructure.Utility
{
    /// <summary>
    /// 提供对象与Json的互换
    /// 忽略对象与子对象间的循环引用  
    /// </summary>
    public static class JsonSerializer
    {
        /// <summary>
        /// 设置时间格式和忽略循环引用
        /// </summary>
        private static JsonSerializerSettings Setting = new JsonSerializerSettings()
        {
            PreserveReferencesHandling = Newtonsoft.Json.PreserveReferencesHandling.None,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            DateFormatString = "yyyy-MM-dd HH:mm:ss"
        };

        /// <summary>
        /// 将对象转换为Json字符串
        /// 支持匿名对象和DataTable对象
        /// </summary>
        /// <param name="source">数据源</param>
        /// <returns></returns>
        public static string Serialize(object source)
        {
            if (source == null)
            {
                return null;
            }
            return JsonConvert.SerializeObject(source, JsonSerializer.Setting);
        }

        /// <summary>
        /// 将对象转换为Json对象
        /// 支持匿名对象和DataTable对象
        /// </summary>
        /// <param name="source">数据源</param>
        /// <param name="dataForamt">时期时间格式</param>
        /// <returns></returns>
        public static string Serialize(object source, string dataForamt)
        {
            if (source == null)
            {
                return null;
            }
            var setting = new JsonSerializerSettings()
            {
                PreserveReferencesHandling = Newtonsoft.Json.PreserveReferencesHandling.None,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                DateFormatString = dataForamt
            };
            return JsonConvert.SerializeObject(source, setting);
        }

        /// <summary>
        /// 将json转换为实体对象
        /// </summary>
        /// <param name="json">json</param>
        /// <param name="entityType">对象类型</param>
        /// <returns></returns>
        public static object Deserialize(string json, Type entityType)
        {
            if (json.IsNullOrEmpty())
            {
                return null;
            }
            return JsonConvert.DeserializeObject(json, entityType);
        }

        /// <summary>
        /// 将json转换为动态类型
        /// </summary>
        /// <param name="json">json</param>
        /// <returns></returns>
        public static dynamic Deserialize(string json)
        {
            if (json.IsNullOrEmpty())
            {
                return null;
            }
            return JsonConvert.DeserializeObject<dynamic>(json);
        }

        /// <summary>
        /// 将json转换为实体对象
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="json">json</param>
        /// <returns></returns>
        public static T Deserialize<T>(string json)
        {
            if (json.IsNullOrEmpty())
            {
                return default(T);
            }
            return JsonConvert.DeserializeObject<T>(json);
        }

        /// <summary>
        /// 尝试将json转换为实体对象
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="json">json</param>
        /// <returns></returns>
        public static T TryDeserialize<T>(string json)
        {
            try
            {
                return Deserialize<T>(json);
            }
            catch (Exception)
            {
                return default(T);
            }
        }

        /// <summary>
        /// 转换为指定类型
        /// </summary>
        /// <typeparam name="T">指定类型</typeparam>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static T ConvertTo<T>(object value)
        {
            if (value == null)
            {
                return default(T);
            }

            var jToken = value as JToken;
            if (jToken != null)
            {
                return jToken.ToObject<T>();
            }
            return Converter.Cast<T>(value);
        }
    }
}