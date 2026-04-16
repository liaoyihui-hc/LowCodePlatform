using LowCode.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace LowCode.Web.Api.Middleware
{
    // 全局模型验证筛选器
    public class ModelValidationFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            // 1. 判断模型是否验证失败
            if (!context.ModelState.IsValid)
            {
                // 2. 提取所有错误信息
                var errors = context.ModelState
                    .Where(x => x.Value?.Errors.Count > 0)
                    .SelectMany(x => x.Value!.Errors)
                    .Select(x => x.ErrorMessage)
                    .ToList();

                var errorMessage = string.Join("；", errors);

                // 3. 直接返回自定义的 ApiResult 格式
                context.Result = new JsonResult(ApiResult<object>.Fail(errorMessage))
                {
                    StatusCode = StatusCodes.Status400BadRequest
                };
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            // 无需处理
        }
    }

}
