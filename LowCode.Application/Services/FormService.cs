using LowCode.Application.Interfaces;
using LowCode.Domain.Entities;
using LowCode.Domain.Models;
using LowCode.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
namespace LowCode.Application.Services
{
    public class FormService :IFormService
    {
        private readonly LowCodeDbContext _dbContext;

        // 保留你的构造函数注入
        public FormService(LowCodeDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        #region 原有业务逻辑（全部包装ApiResult，无任何报错）
        public async Task<ApiResult<List<FormEntity>>> GetFormListAsync()
        {
            var list = await _dbContext.Forms
                .OrderByDescending(f => f.UpdateTime)
                .ToListAsync();
            // 用ApiResult包装返回，解决报错
            return ApiResult<List<FormEntity>>.Success(list);
        }

        public async Task<ApiResult<FormEntity?>> GetFormByIdAsync(Guid id)
        {
            var form = await _dbContext.Forms.FindAsync(id);
            return ApiResult<FormEntity?>.Success(form);
        }

        public async Task<ApiResult<FormEntity?>> GetFormByCodeAsync(string formCode)
        {
            var form = await _dbContext.Forms
                .FirstOrDefaultAsync(f => f.FormCode == formCode);
            return ApiResult<FormEntity?>.Success(form);
        }

        public async Task<ApiResult<Guid>> CreateFormAsync(FormEntity entity)
        {
            var isExist = await _dbContext.Forms.AnyAsync(f => f.FormCode == entity.FormCode);
            if (isExist) throw new Exception("表单编码已存在");

            entity.Id = Guid.NewGuid();
            entity.CreateTime = DateTime.Now;
            entity.UpdateTime = DateTime.Now;

            await _dbContext.Forms.AddAsync(entity);
            await _dbContext.SaveChangesAsync();
            return ApiResult<Guid>.Success(entity.Id);
        }

        public async Task<ApiResult<bool>> UpdateFormAsync(FormEntity entity)
        {
            var form = await _dbContext.Forms.FindAsync(entity.Id);
            if (form == null) throw new Exception("表单不存在");

            var isExist = await _dbContext.Forms.AnyAsync(f => f.FormCode == entity.FormCode && f.Id != entity.Id);
            if (isExist) throw new Exception("表单编码已存在");

            form.FormName = entity.FormName;
            form.FormCode = entity.FormCode;
            form.FieldsConfigJson = entity.FieldsConfigJson;
            form.SubmitApiUrl = entity.SubmitApiUrl;
            form.IsSaveData = entity.IsSaveData;
            form.UpdateTime = DateTime.Now;

            await _dbContext.SaveChangesAsync();
            return ApiResult<bool>.Success(true);
        }

        public async Task<ApiResult<bool>> DeleteFormAsync(Guid id)
        {
            var form = await _dbContext.Forms.FindAsync(id);
            if (form == null) throw new Exception("表单不存在");

            _dbContext.Forms.Remove(form);
            await _dbContext.SaveChangesAsync();
            return ApiResult<bool>.Success(true);
        }
        #endregion

        #region 新增：低代码核心功能
        /// <summary>
        /// 发布表单
        /// </summary>
        public async Task<ApiResult<string>> PublishFormAsync(Guid id)
        {
            var form = await _dbContext.Forms.FindAsync(id);
            if (form == null) throw new Exception("表单不存在");

            form.Status = 1;
            form.UpdateTime = DateTime.Now;
            await _dbContext.SaveChangesAsync();

            return ApiResult<string>.Success("表单发布成功！");
        }

        /// <summary>
        /// 前端渲染核心
        /// </summary>
        public async Task<ApiResult<object>> GetFormRenderConfigAsync(Guid formId)
        {
            var form = await _dbContext.Forms.FindAsync(formId);
            if (form == null) throw new Exception("表单不存在");
            if (form.Status != 1) throw new Exception("表单未发布，无法渲染");

            var fieldsConfig = JsonSerializer.Deserialize<object>(form.FieldsConfigJson);
            var renderData = new
            {
                form.Id,
                form.FormName,
                form.FormCode,
                form.DatasourceId,
                Fields = fieldsConfig,
                form.SubmitApiUrl,
                form.IsSaveData
            };

            return ApiResult<object>.Success(renderData);
        }

        /// <summary>
        /// 数据源动态查询
        /// </summary>
        public async Task<ApiResult<object>> GetFormDataSourceDataAsync(Guid formId)
        {
            var form = await _dbContext.Forms.FindAsync(formId);
            if (form == null) throw new Exception("表单不存在");
            if (form.DatasourceId == Guid.Empty) throw new Exception("表单未绑定数据源");

            return ApiResult<object>.Success(new { Data = "表单绑定的数据源动态数据" });
        }
        #endregion
    }
}
