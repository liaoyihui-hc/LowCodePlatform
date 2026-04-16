﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using LowCode.Domain.Entities;
using LowCode.Infrastructure;
using LowCode.Application.Interfaces;
using LowCode.Domain.Models;

namespace LowCode.Application.Services
{
    public class PageService : IPageService
    {
        private readonly LowCodeDbContext _dbContext;

        public PageService(LowCodeDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ApiResult<List<PageEntity>>> GetPageListAsync()
        {
            var list = await _dbContext.Pages
                .OrderByDescending(p => p.UpdateTime)
                .ToListAsync();
            return ApiResult<List<PageEntity>>.Success(list);
        }

        public async Task<ApiResult<PageEntity?>> GetPageByIdAsync(Guid id)
        {
            var page = await _dbContext.Pages.FindAsync(id);
            return ApiResult<PageEntity?>.Success(page);
        }

        public async Task<ApiResult<PageEntity?>> GetPublishedPageByNameAsync(string pageName)
        {
            var page = await _dbContext.Pages
                .FirstOrDefaultAsync(p => p.PageName == pageName && p.PublishStatus == 1);
            return ApiResult<PageEntity?>.Success(page);
        }

        public async Task<ApiResult<Guid>> CreatePageAsync(PageEntity entity)
        {
            var isExist = await _dbContext.Pages.AnyAsync(p => p.PageName == entity.PageName);
            if (isExist)
            {
                throw new Exception("页面名称已存在，请更换名称");
            }

            entity.Id = Guid.NewGuid();
            entity.CreateTime = DateTime.Now;
            entity.UpdateTime = DateTime.Now;

            await _dbContext.Pages.AddAsync(entity);
            await _dbContext.SaveChangesAsync();

            return ApiResult<Guid>.Success(entity.Id);
        }

        public async Task<ApiResult<bool>> UpdatePageAsync(PageEntity entity)
        {
            var page = await _dbContext.Pages.FindAsync(entity.Id);
            if (page == null)
            {
                throw new Exception("页面不存在");
            }

            var isExist = await _dbContext.Pages
                .AnyAsync(p => p.PageName == entity.PageName && p.Id != entity.Id);
            if (isExist)
            {
                throw new Exception("页面名称已存在，请更换名称");
            }

            page.PageName = entity.PageName;
            page.PageTitle = entity.PageTitle;
            page.ComponentTreeJson = entity.ComponentTreeJson;
            page.UpdateTime = DateTime.Now;

            _dbContext.Pages.Update(page);
            await _dbContext.SaveChangesAsync();

            return ApiResult<bool>.Success(true);
        }

        public async Task<ApiResult<bool>> DeletePageAsync(Guid id)
        {
            var page = await _dbContext.Pages.FindAsync(id);
            if (page == null)
            {
                throw new Exception("页面不存在");
            }

            _dbContext.Pages.Remove(page);
            await _dbContext.SaveChangesAsync();

            return ApiResult<bool>.Success(true);
        }

        public async Task<ApiResult<bool>> PublishPageAsync(Guid id, int publishStatus)
        {
            var page = await _dbContext.Pages.FindAsync(id);
            if (page == null)
            {
                throw new Exception("页面不存在");
            }

            page.PublishStatus = publishStatus;
            page.UpdateTime = DateTime.Now;

            _dbContext.Pages.Update(page);
            await _dbContext.SaveChangesAsync();

            return ApiResult<bool>.Success(true);
        }
    }
}
