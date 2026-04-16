﻿using LowCode.Domain.Entities;
using LowCode.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace LowCode.Application.Interfaces
{
    public interface IPageService
    {
        /// <summary>
        /// 获取所有页面列表
        /// </summary>
        Task<ApiResult<List<PageEntity>>> GetPageListAsync();

        /// <summary>
        /// 根据ID获取页面详情
        /// </summary>
        Task<ApiResult<PageEntity?>> GetPageByIdAsync(Guid id);

        /// <summary>
        /// 根据页面名称获取已发布页面（前端渲染用）
        /// </summary>
        Task<ApiResult<PageEntity?>> GetPublishedPageByNameAsync(string pageName);

        /// <summary>
        /// 创建页面
        /// </summary>
        Task<ApiResult<Guid>> CreatePageAsync(PageEntity entity);

        /// <summary>
        /// 更新页面配置
        /// </summary>
        Task<ApiResult<bool>> UpdatePageAsync(PageEntity entity);

        /// <summary>
        /// 删除页面
        /// </summary>
        Task<ApiResult<bool>> DeletePageAsync(Guid id);

        /// <summary>
        /// 发布/取消发布页面
        /// </summary>
        Task<ApiResult<bool>> PublishPageAsync(Guid id, int publishStatus);
    }
}
