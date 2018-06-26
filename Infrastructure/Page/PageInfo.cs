using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Infrastructure.Page
{
    /// <summary>
    /// 分页信息   
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    [Serializable]
    public class PageInfo<T> : IPageInfo<T> where T : class
    {
        /// <summary>
        /// 当前页面数据
        /// </summary>
        private IEnumerable<T> pageEntities { get; set; }

        /// <summary>
        /// 页面索引
        /// </summary>
        public int PageIndex { get; set; }

        /// <summary>
        /// 页面大小
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// 总记录条数
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// 获取模型类型
        /// </summary>
        public T Model { get; private set; }

        /// <summary>
        /// 分页信息
        /// </summary>
        /// <param name="totalCount">所有条目</param>
        /// <param name="pageEntities">分页数据</param>
        public PageInfo(int totalCount, IEnumerable<T> pageEntities)
        {
            this.TotalCount = totalCount;
            this.pageEntities = pageEntities;
        }

        /// <summary>
        /// 总记录条目
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("TotalCount={0}", this.TotalCount);
        }

        /// <summary>
        /// 获取字段名
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="keySelector"></param>
        /// <returns></returns>
        public string Field<TKey>(Expression<Func<T, TKey>> keySelector)
        {
            return keySelector.Body.OfType<MemberExpression>().Member.Name;
        }

        /// <summary>
        /// 将分页数据内容映射为其它类型
        /// </summary>
        /// <typeparam name="TNew"></typeparam>
        /// <param name="selector"></param>
        /// <returns></returns>
        public PageInfo<TNew> MapAs<TNew>(Func<T, TNew> selector) where TNew : class
        {
            var models = this.pageEntities.Select(selector).ToArray();
            return new PageInfo<TNew>(this.TotalCount, models) { PageIndex = this.PageIndex, PageSize = this.PageSize };
        }

        #region 接口实现
        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return this.pageEntities.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.pageEntities.GetEnumerator();
        }
        #endregion
    }
}
