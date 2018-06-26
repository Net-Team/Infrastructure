using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Restful
{
    /// <summary>
    /// WeiXinApi接口返回对象接口
    /// </summary>
    public interface IRestResult
    {
        /// <summary>
        /// 错误码
        /// </summary>
        int Code { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        bool State { get; set; }

        /// <summary>
        /// 数据类容
        /// </summary>
        object Data { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class RestResult : IRestResult
    {
        /// <summary>
        /// 错误码
        /// </summary>
        public int Code { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public bool State { get; set; }

        /// <summary>
        /// 数据类容
        /// </summary>
        public object Data { get; set; }

        public static RestResult True(object data)
        {
            return new RestResult { Data = data, State = true };
        }

        public static RestResult False(int code, string msg)
        {
            return new RestResult { Data = null, State = false, Code = code };
        }
    }



    /// <summary>
    /// 微信接口返回实体
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RestResult<T> : IRestResult where T : class
    {
        /// <summary>
        /// 错误码
        /// </summary>
        public int Code { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public bool State { get; set; }

        /// <summary>
        /// 数据类容
        /// </summary>
        public T Data { get; set; }


        object IRestResult.Data
        {
            get
            {
                return this.Data;
            }
            set
            {
                this.Data = (T)value;
            }
        }

        public static RestResult<T> True(T data)
        {
            return new RestResult<T> { Data = data, State = true };
        }

        public static RestResult<T> False(int code, string msg)
        {
            return new RestResult<T> { Data = null, State = false, Code = code };
        }
    }
}
