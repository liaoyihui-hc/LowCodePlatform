using LowCode.Domain.Entities;
using LowCode.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace LowCode.Application.Interfaces
{
   public interface IFormService
    {

        // 🔥 全部统一为 ApiResult 返回值，和 FormService 完全匹配
        Task<ApiResult<List<FormEntity>>> GetFormListAsync();
        Task<ApiResult<FormEntity?>> GetFormByIdAsync(Guid id);
        Task<ApiResult<FormEntity?>> GetFormByCodeAsync(string formCode);
        Task<ApiResult<Guid>> CreateFormAsync(FormEntity entity);
        Task<ApiResult<bool>> UpdateFormAsync(FormEntity entity);
        Task<ApiResult<bool>> DeleteFormAsync(Guid id);

        // 新增低代码核心方法
        Task<ApiResult<string>> PublishFormAsync(Guid id);
        Task<ApiResult<object>> GetFormRenderConfigAsync(Guid formId);
        Task<ApiResult<object>> GetFormDataSourceDataAsync(Guid formId);
    }
}
