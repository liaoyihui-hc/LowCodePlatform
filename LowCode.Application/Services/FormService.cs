using LowCode.Application.Interfaces;
using LowCode.Domain.Entities;
using LowCode.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
namespace LowCode.Application.Services
{
    public class FormService:IFormService
    {

        private readonly LowCodeDbContext _dbContext;

        public FormService(LowCodeDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<FormEntity>> GetFormListAsync()
        {
            return await _dbContext.Forms
                .OrderByDescending(f => f.UpdateTime)
                .ToListAsync();
        }

        public async Task<FormEntity?> GetFormByIdAsync(Guid id)
        {
            return await _dbContext.Forms.FindAsync(id);
        }

        public async Task<FormEntity?> GetFormByCodeAsync(string formCode)
        {
            return await _dbContext.Forms
                .FirstOrDefaultAsync(f => f.FormCode == formCode);
        }

        public async Task<Guid> CreateFormAsync(FormEntity entity)
        {
            var isExist = await _dbContext.Forms.AnyAsync(f => f.FormCode == entity.FormCode);
            if (isExist) throw new Exception("表单编码已存在");

            entity.Id = Guid.NewGuid();
            entity.CreateTime = DateTime.Now;
            entity.UpdateTime = DateTime.Now;

            await _dbContext.Forms.AddAsync(entity);
            await _dbContext.SaveChangesAsync();
            return entity.Id;
        }

        public async Task<bool> UpdateFormAsync(FormEntity entity)
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
            return true;
        }

        public async Task<bool> DeleteFormAsync(Guid id)
        {
            var form = await _dbContext.Forms.FindAsync(id);
            if (form == null) throw new Exception("表单不存在");

            _dbContext.Forms.Remove(form);
            await _dbContext.SaveChangesAsync();
            return true;
        }
    }
}
