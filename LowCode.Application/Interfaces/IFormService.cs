using LowCode.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace LowCode.Application.Interfaces
{
   public interface IFormService
    {

        Task<List<FormEntity>> GetFormListAsync();
        Task<FormEntity?> GetFormByIdAsync(Guid id);
        Task<FormEntity?> GetFormByCodeAsync(string formCode);
        Task<Guid> CreateFormAsync(FormEntity entity);
        Task<bool> UpdateFormAsync(FormEntity entity);
        Task<bool> DeleteFormAsync(Guid id);
    }
}
