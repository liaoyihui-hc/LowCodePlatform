using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using LowCode.Domain.Entities;
using LowCode.Infrastructure;
using LowCode.Application.Interfaces;
namespace LowCode.Application.Services
{
    
    public class PageService:IPageService
    {
        // 注入数据库上下文，不用手动new，依赖注入自动处理
    private readonly LowCodeDbContext _dbContext;

    public PageService(LowCodeDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<PageEntity>> GetPageListAsync()
    {
        // 按更新时间倒序，最新修改的排在前面
        return await _dbContext.Pages
            .OrderByDescending(p => p.UpdateTime)
            .ToListAsync();
    }

    public async Task<PageEntity?> GetPageByIdAsync(Guid id)
    {
        return await _dbContext.Pages.FindAsync(id);
    }

    public async Task<PageEntity?> GetPublishedPageByNameAsync(string pageName)
    {
        // 只返回已发布的页面，前端渲染用
        return await _dbContext.Pages
            .FirstOrDefaultAsync(p => p.PageName == pageName && p.PublishStatus == 1);
    }

    public async Task<Guid> CreatePageAsync(PageEntity entity)
    {
        // 业务校验：页面名称不能重复
        var isExist = await _dbContext.Pages.AnyAsync(p => p.PageName == entity.PageName);
        if (isExist)
        {
            throw new Exception("页面名称已存在，请更换名称");
        }

        // 初始化字段
        entity.Id = Guid.NewGuid();
        entity.CreateTime = DateTime.Now;
        entity.UpdateTime = DateTime.Now;

        // 写入数据库
        await _dbContext.Pages.AddAsync(entity);
        await _dbContext.SaveChangesAsync();

        return entity.Id;
    }

    public async Task<bool> UpdatePageAsync(PageEntity entity)
    {
        // 校验页面是否存在
        var page = await _dbContext.Pages.FindAsync(entity.Id);
        if (page == null)
        {
            throw new Exception("页面不存在");
        }

        // 校验页面名称是否和其他页面重复
        var isExist = await _dbContext.Pages
            .AnyAsync(p => p.PageName == entity.PageName && p.Id != entity.Id);
        if (isExist)
        {
            throw new Exception("页面名称已存在，请更换名称");
        }

        // 更新字段，只更新允许修改的字段
        page.PageName = entity.PageName;
        page.PageTitle = entity.PageTitle;
        page.ComponentTreeJson = entity.ComponentTreeJson;
        page.UpdateTime = DateTime.Now;

        // 保存到数据库
        _dbContext.Pages.Update(page);
        await _dbContext.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeletePageAsync(Guid id)
    {
        var page = await _dbContext.Pages.FindAsync(id);
        if (page == null)
        {
            throw new Exception("页面不存在");
        }

        _dbContext.Pages.Remove(page);
        await _dbContext.SaveChangesAsync();

        return true;
    }

    public async Task<bool> PublishPageAsync(Guid id, int publishStatus)
    {
        var page = await _dbContext.Pages.FindAsync(id);
        if (page == null)
        {
            throw new Exception("页面不存在");
        }

        // 更新发布状态
        page.PublishStatus = publishStatus;
        page.UpdateTime = DateTime.Now;

        _dbContext.Pages.Update(page);
        await _dbContext.SaveChangesAsync();

        return true;
    }
    }
}
