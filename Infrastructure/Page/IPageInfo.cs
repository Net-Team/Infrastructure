using System.Collections;
using System.Collections.Generic;

namespace Infrastructure.Page
{
    /// <summary>
    /// 分页接口
    /// </summary>
    public interface IPageInfo : IEnumerable
    {
        /// <summary>
        /// 页面索引
        /// </summary>
        int PageIndex { get; set; }

        /// <summary>
        /// 页面大小
        /// </summary>
        int PageSize { get; set; }

        /// <summary>
        /// 总记录条数
        /// </summary>
        int TotalCount { get; set; }
    }

    /// <summary>
    /// 分页接口
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IPageInfo<out T> : IPageInfo, IEnumerable<T> where T : class
    {
        /// <summary>
        /// 获取模型类型
        /// </summary>
        T Model { get; }
    }
}
