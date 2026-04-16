﻿using LowCode.Domain.Entities;
using LowCode.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace LowCode.Application.Interfaces
{
    public interface IComponentMetaService
    {
        Task<ApiResult<List<ComponentMetaEntity>>> GetComponentListAsync();
        Task<ApiResult<ComponentMetaEntity?>> GetComponentByIdAsync(Guid id);
        Task<ApiResult<ComponentMetaEntity?>> GetComponentByTypeAsync(string componentType);
        Task<ApiResult<Guid>> CreateComponentAsync(ComponentMetaEntity entity);
        Task<ApiResult<bool>> UpdateComponentAsync(ComponentMetaEntity entity);
        Task<ApiResult<bool>> DeleteComponentAsync(Guid id);
        Task<ApiResult<bool>> ToggleComponentStatusAsync(Guid id, int isEnable);
    }
}
