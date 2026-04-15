using System;
using System.Collections.Generic;
using System.Text;

namespace LowCode.Domain.Models
{
        /// <summary>
        /// 通用API统一返回模型（非泛型）
        /// </summary>
        public class ApiResult
        {
            /// <summary>
            /// 状态码：200=成功，500=失败
            /// </summary>
            public int Code { get; set; }

            /// <summary>
            /// 返回消息
            /// </summary>
            public string Msg { get; set; }

            /// <summary>
            /// 返回数据
            /// </summary>
            public object? Data { get; set; }

            #region 静态快捷方法
            /// <summary>
            /// 成功返回
            /// </summary>
            public static ApiResult Success(object? data = null, string msg = "操作成功")
            {
                return new ApiResult { Code = 200, Msg = msg, Data = data };
            }

            /// <summary>
            /// 失败返回
            /// </summary>
            public static ApiResult Fail(string msg = "操作失败", object? data = null)
            {
                return new ApiResult { Code = 500, Msg = msg, Data = data };
            }
            #endregion
        }

        /// <summary>
        /// 泛型API统一返回模型（强类型，推荐使用）
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        public class ApiResult<T>
        {
            /// <summary>
            /// 状态码：200=成功，500=失败
            /// </summary>
            public int Code { get; set; }

            /// <summary>
            /// 返回消息
            /// </summary>
            public string Msg { get; set; }

            /// <summary>
            /// 强类型返回数据
            /// </summary>
            public T? Data { get; set; }

            #region 静态快捷方法
            /// <summary>
            /// 成功返回
            /// </summary>
            public static ApiResult<T> Success(T? data, string msg = "操作成功")
            {
                return new ApiResult<T> { Code = 200, Msg = msg, Data = data };
            }

            /// <summary>
            /// 失败返回
            /// </summary>
            public static ApiResult<T> Fail(string msg = "操作失败", T? data = default)
            {
                return new ApiResult<T> { Code = 500, Msg = msg, Data = data };
            }
            #endregion
        }
    
}
