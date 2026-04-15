using System;
using System.Collections.Generic;
using System.Text;

namespace LowCode.Domain.Models
{
    public class ApiResponse<T>
    {

        public int Code { get; set; }
        public string Msg { get; set; } = string.Empty;
        public T? Data { get; set; }

        // 静态快速创建方法
        public static ApiResponse<T> Success(T data, string msg = "操作成功")
        {
            return new ApiResponse<T> { Code = 200, Msg = msg, Data = data };
        }

        public static ApiResponse<T> Fail(string msg, int code = 400)
        {
            return new ApiResponse<T> { Code = code, Msg = msg };
        }
    }
}
