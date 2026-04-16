﻿using LowCode.Application.Interfaces;
using LowCode.Domain.Entities;
using LowCode.Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LowCode.Web.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PagesController : ControllerBase
    {
        private readonly IPageService _pageService;

        public PagesController(IPageService pageService)
        {
            _pageService = pageService;
        }

        /// <summary>
        /// 获取所有页面列表
        /// </summary>
        /// <returns>页面列表</returns>
        [HttpGet("list")]
        public async Task<ApiResult<List<PageEntity>>> GetPageList()
        {
            var list = await _pageService.GetPageListAsync();
            return ApiResult<List<PageEntity>>.Success(list);
        }

        /// <summary>
        /// 根据ID获取页面详情
        /// </summary>
        /// <param name="id">页面唯一ID</param>
        /// <returns>页面详情</returns>
        [HttpGet("detail/{id}")]
        public async Task<ApiResult<PageEntity?>> GetPageById(Guid id)
        {
            var page = await _pageService.GetPageByIdAsync(id);
            return ApiResult<PageEntity?>.Success(page);
        }

        /// <summary>
        /// 根据页面名称获取已发布页面（前端渲染用）
        /// </summary>
        /// <param name="pageName">页面名称</param>
        /// <returns>页面详情</returns>
        [HttpGet("render/{pageName}")]
        public async Task<ApiResult<PageEntity?>> GetPublishedPage(string pageName)
        {
            var page = await _pageService.GetPublishedPageByNameAsync(pageName);
            return ApiResult<PageEntity?>.Success(page);
        }

        /// <summary>
        /// 创建新页面
        /// </summary>
        /// <param name="entity">页面配置实体</param>
        /// <returns>页面ID</returns>
        [HttpPost("create")]
        public async Task<ApiResult<Guid>> CreatePage([FromBody] PageEntity entity)
        {
            var pageId = await _pageService.CreatePageAsync(entity);
            return ApiResult<Guid>.Success(pageId);
        }

        /// <summary>
        /// 更新页面配置
        /// </summary>
        /// <param name="entity">页面配置实体</param>
        /// <returns>更新结果</returns>
        [HttpPut("update")]
        public async Task<ApiResult<bool>> UpdatePage([FromBody] PageEntity entity)
        {
            var result = await _pageService.UpdatePageAsync(entity);
            return ApiResult<bool>.Success(result);
        }

        /// <summary>
        /// 删除页面
        /// </summary>
        /// <param name="id">页面唯一ID</param>
        /// <returns>删除结果</returns>
        [HttpDelete("delete/{id}")]
        public async Task<ApiResult<bool>> DeletePage(Guid id)
        {
            var result = await _pageService.DeletePageAsync(id);
            return ApiResult<bool>.Success(result);
        }

        /// <summary>
        /// 发布/取消发布页面
        /// </summary>
        /// <param name="id">页面唯一ID</param>
        /// <param name="publishStatus">发布状态：0-草稿 1-已发布</param>
        /// <returns>操作结果</returns>
        [HttpPut("publish/{id}")]
        public async Task<ApiResult<bool>> PublishPage(Guid id, [FromQuery] int publishStatus)
        {
            var result = await _pageService.PublishPageAsync(id, publishStatus);
            return ApiResult<bool>.Success(result);
        }
    }
}
