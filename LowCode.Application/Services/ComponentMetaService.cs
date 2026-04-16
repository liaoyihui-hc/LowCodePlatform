﻿using LowCode.Domain.Entities;
using LowCode.Infrastructure;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using LowCode.Application.Interfaces;
using LowCode.Domain.Models;
using Microsoft.Extensions.Caching.Memory;

namespace LowCode.Application.Services
{
    public class ComponentMetaService : IComponentMetaService
    {
        private readonly LowCodeDbContext _dbContext;
        private readonly IMemoryCache _memoryCache;
        private const string ComponentListCacheKey = "ComponentMeta_List";
        private const int CacheExpirationMinutes = 30;

        public ComponentMetaService(LowCodeDbContext dbContext, IMemoryCache memoryCache)
        {
            _dbContext = dbContext;
            _memoryCache = memoryCache;
        }

        public async Task<ApiResult<List<ComponentMetaEntity>>> GetComponentListAsync()
        {
            if (_memoryCache.TryGetValue(ComponentListCacheKey, out List<ComponentMetaEntity> cachedComponents))
            {
                return ApiResult<List<ComponentMetaEntity>>.Success(cachedComponents);
            }

            var list = await _dbContext.ComponentMetas
                .Where(c => c.IsEnable == 1)
                .OrderBy(c => c.Sort)
                .ToListAsync();

            _memoryCache.Set(ComponentListCacheKey, list, TimeSpan.FromMinutes(CacheExpirationMinutes));
            return ApiResult<List<ComponentMetaEntity>>.Success(list);
        }

        public async Task<ApiResult<ComponentMetaEntity?>> GetComponentByIdAsync(Guid id)
        {
            var cacheKey = $"ComponentMeta_{id}";
            if (_memoryCache.TryGetValue(cacheKey, out ComponentMetaEntity? cachedComponent))
            {
                return ApiResult<ComponentMetaEntity?>.Success(cachedComponent);
            }

            var component = await _dbContext.ComponentMetas.FindAsync(id);
            if (component != null)
            {
                _memoryCache.Set(cacheKey, component, TimeSpan.FromMinutes(CacheExpirationMinutes));
            }
            return ApiResult<ComponentMetaEntity?>.Success(component);
        }

        public async Task<ApiResult<ComponentMetaEntity?>> GetComponentByTypeAsync(string componentType)
        {
            var cacheKey = $"ComponentMeta_Type_{componentType}";
            if (_memoryCache.TryGetValue(cacheKey, out ComponentMetaEntity? cachedComponent))
            {
                return ApiResult<ComponentMetaEntity?>.Success(cachedComponent);
            }

            var component = await _dbContext.ComponentMetas
                .FirstOrDefaultAsync(c => c.ComponentType == componentType && c.IsEnable == 1);
            if (component != null)
            {
                _memoryCache.Set(cacheKey, component, TimeSpan.FromMinutes(CacheExpirationMinutes));
            }
            return ApiResult<ComponentMetaEntity?>.Success(component);
        }

        public async Task<ApiResult<Guid>> CreateComponentAsync(ComponentMetaEntity entity)
        {
            var isExist = await _dbContext.ComponentMetas.AnyAsync(c => c.ComponentType == entity.ComponentType);
            if (isExist) throw new Exception("组件类型已存在");

            entity.Id = Guid.NewGuid();
            await _dbContext.ComponentMetas.AddAsync(entity);
            await _dbContext.SaveChangesAsync();

            ClearComponentCache();
            return ApiResult<Guid>.Success(entity.Id);
        }

        public async Task<ApiResult<bool>> UpdateComponentAsync(ComponentMetaEntity entity)
        {
            var component = await _dbContext.ComponentMetas.FindAsync(entity.Id);
            if (component == null) throw new Exception("组件不存在");

            var isExist = await _dbContext.ComponentMetas.AnyAsync(c => c.ComponentType == entity.ComponentType && c.Id != entity.Id);
            if (isExist) throw new Exception("组件类型已存在");

            component.ComponentType = entity.ComponentType;
            component.ComponentName = entity.ComponentName;
            component.GroupName = entity.GroupName;
            component.DefaultPropsJson = entity.DefaultPropsJson;
            component.Icon = entity.Icon;
            component.Sort = entity.Sort;

            await _dbContext.SaveChangesAsync();

            ClearComponentCache();
            return ApiResult<bool>.Success(true);
        }

        public async Task<ApiResult<bool>> DeleteComponentAsync(Guid id)
        {
            var component = await _dbContext.ComponentMetas.FindAsync(id);
            if (component == null) throw new Exception("组件不存在");

            _dbContext.ComponentMetas.Remove(component);
            await _dbContext.SaveChangesAsync();

            ClearComponentCache();
            return ApiResult<bool>.Success(true);
        }

        public async Task<ApiResult<bool>> ToggleComponentStatusAsync(Guid id, int isEnable)
        {
            var component = await _dbContext.ComponentMetas.FindAsync(id);
            if (component == null) throw new Exception("组件不存在");

            component.IsEnable = isEnable;
            await _dbContext.SaveChangesAsync();

            ClearComponentCache();
            return ApiResult<bool>.Success(true);
        }

        private void ClearComponentCache()
        {
            _memoryCache.Remove(ComponentListCacheKey);
        }
    }
}
