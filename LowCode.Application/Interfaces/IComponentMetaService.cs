using LowCode.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace LowCode.Application.Interfaces
{
    public interface IComponentMetaService
    {
        Task<List<ComponentMetaEntity>> GetComponentListAsync();
        Task<ComponentMetaEntity?> GetComponentByIdAsync(Guid id);
        Task<ComponentMetaEntity?> GetComponentByTypeAsync(string componentType);
        Task<Guid> CreateComponentAsync(ComponentMetaEntity entity);
        Task<bool> UpdateComponentAsync(ComponentMetaEntity entity);
        Task<bool> DeleteComponentAsync(Guid id);
        Task<bool> ToggleComponentStatusAsync(Guid id, int isEnable);
    }
}
