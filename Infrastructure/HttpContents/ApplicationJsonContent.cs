using System.Net.Http;
using System.Text;

namespace Infrastructure.HttpContents
{
    /// <summary>
    /// 表示application/json的http内容
    /// </summary>
    public class ApplicationJsonContent : StringContent
    {
        /// <summary>
        /// application/json的http内容
        /// </summary>
        /// <param name="model">内容模型</param>
        public ApplicationJsonContent(object model)
            : base(GetJson(model), Encoding.UTF8, "application/json")
        {
        }

        /// <summary>
        /// 获取json内容
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private static string GetJson(object model)
        {
            if (model == null)
            {
                return null;
            }
            var json = Utility.JsonSerializer.Serialize(model);
            return json;
        }
    }
}
