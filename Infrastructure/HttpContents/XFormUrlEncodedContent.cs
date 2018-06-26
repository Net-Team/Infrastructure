using Infrastructure.Reflection;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;


namespace Infrastructure.HttpContents
{
    /// <summary>
    /// 表示x-www-form-urlencoded的http内容
    /// </summary>
    public class XFormUrlEncodedContent : StringContent
    {
        /// <summary>
        /// 表单内容
        /// </summary>
        private readonly string formString;

        /// <summary>
        /// x-www-form-urlencoded的http内容
        /// </summary>
        /// <param name="form">表单内容</param>
        public XFormUrlEncodedContent(string form)
            : base(form, Encoding.UTF8, "application/x-www-form-urlencoded")
        {
            this.formString = form;
        }

        /// <summary>
        /// x-www-form-urlencoded的http内容
        /// </summary>
        /// <param name="form">表单模型</param>
        public XFormUrlEncodedContent(object form)
            : this(GetFormString(form, true))
        {
        }

        /// <summary>
        /// 转换为字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.formString;
        }


        /// <summary>
        /// 获取简单对象的表单键值对
        /// </summary>
        /// <param name="model"></param>
        /// <param name="camelCase">camelCase</param>
        /// <returns></returns>
        public static string GetFormString(object model, bool camelCase = false)
        {
            if (model == null)
            {
                return string.Empty;
            }

            var token = model as JToken;
            var keyValues = token == null ? GetObjectProperties(model) : GetJTokenProperties(token);
            return XFormUrlEncodedContent.GetFormString(keyValues, camelCase);
        }

        /// <summary>
        /// 获取简单对象的表单键值对
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static string GetFormString(IEnumerable<KeyValuePair<string, object>> keyValues, bool camelCase = false)
        {
            if (keyValues == null)
            {
                return string.Empty;
            }

            var forms = keyValues.Select(kv =>
            {
                var value = kv.Value;
                var valueString = value == null ? null : value.ToString();
                var valueEncoded = System.Web.HttpUtility.UrlEncode(valueString, Encoding.UTF8);
                var key = camelCase ? kv.Key.CamelCase() : kv.Key;
                return string.Format("{0}={1}", key, valueEncoded);
            });

            var formValue = string.Join("&", forms);
            return formValue;
        }

        /// <summary>
        /// 获取简单对象属性与值
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private static IEnumerable<KeyValuePair<string, object>> GetObjectProperties(object model)
        {
            foreach (var p in Property.GetProperties(model.GetType()))
            {
                var pValue = p.GetValue(model);
                if (pValue != null && p.Info.PropertyType.IsEnum)
                {
                    pValue = (int)pValue;
                }
                yield return new KeyValuePair<string, object>(p.Name, pValue);
            }
        }

        /// <summary>
        /// 获取JToken属性与值
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        private static IEnumerable<KeyValuePair<string, object>> GetJTokenProperties(JToken token)
        {
            if (token == null || token.HasValues == false)
            {
                yield break;
            }

            var node = token.First;
            var pNode = node as JProperty;
            if (pNode != null)
            {
                yield return new KeyValuePair<string, object>(pNode.Name, pNode.Value);
            }

            while ((node = node.Next) != null)
            {
                pNode = node as JProperty;
                yield return new KeyValuePair<string, object>(pNode.Name, pNode.Value);
            }
        }
    }
}
