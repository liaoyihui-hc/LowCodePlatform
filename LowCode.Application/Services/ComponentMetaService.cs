using LowCode.Domain.Entities;
using LowCode.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using LowCode.Application.Interfaces;
namespace LowCode.Application.Services
{
   public class ComponentMetaService :IComponentMetaService

    {

        private readonly LowCodeDbContext _dbContext;

        public ComponentMetaService(LowCodeDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<ComponentMetaEntity>> GetComponentListAsync()
        {
            return await _dbContext.ComponentMetas
                .Where(c => c.IsEnable == 1)
                .OrderBy(c => c.Sort)
                .ToListAsync();
        }

        public async Task<ComponentMetaEntity?> GetComponentByIdAsync(Guid id)
        {
            return await _dbContext.ComponentMetas.FindAsync(id);
        }

        public async Task<ComponentMetaEntity?> GetComponentByTypeAsync(string componentType)
        {
            return await _dbContext.ComponentMetas
                .FirstOrDefaultAsync(c => c.ComponentType == componentType && c.IsEnable == 1);
        }

        public async Task<Guid> CreateComponentAsync(ComponentMetaEntity entity)
        {
            var isExist = await _dbContext.ComponentMetas.AnyAsync(c => c.ComponentType == entity.ComponentType);
            if (isExist) throw new Exception("组件类型已存在");

            entity.Id = Guid.NewGuid();
            await _dbContext.ComponentMetas.AddAsync(entity);
            await _dbContext.SaveChangesAsync();
            return entity.Id;
        }

        public async Task<bool> UpdateComponentAsync(ComponentMetaEntity entity)
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
            return true;
        }

        public async Task<bool> DeleteComponentAsync(Guid id)
        {
            var component = await _dbContext.ComponentMetas.FindAsync(id);
            if (component == null) throw new Exception("组件不存在");

            _dbContext.ComponentMetas.Remove(component);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ToggleComponentStatusAsync(Guid id, int isEnable)
        {
            var component = await _dbContext.ComponentMetas.FindAsync(id);
            if (component == null) throw new Exception("组件不存在");

            component.IsEnable = isEnable;
            await _dbContext.SaveChangesAsync();
            return true;
        }
    }
}
