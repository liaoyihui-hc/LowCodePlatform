using LowCode.Application.Interfaces;
using LowCode.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LowCode.Web.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PagesController : ControllerBase
    {
        // 注入页面服务，不用手动new
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
        public async Task<IActionResult> GetPageList()
        {
            try
            {
                var list = await _pageService.GetPageListAsync();
                return Ok(new { Code = 200, Msg = "获取成功", Data = list });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Code = 400, Msg = ex.Message });
            }
        }

        /// <summary>
        /// 根据ID获取页面详情
        /// </summary>
        /// <param name="id">页面唯一ID</param>
        /// <returns>页面详情</returns>
        [HttpGet("detail/{id}")]
        public async Task<IActionResult> GetPageById(Guid id)
        {
            try
            {
                var page = await _pageService.GetPageByIdAsync(id);
                if (page == null)
                {
                    return NotFound(new { Code = 404, Msg = "页面不存在" });
                }
                return Ok(new { Code = 200, Msg = "获取成功", Data = page });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Code = 400, Msg = ex.Message });
            }
        }

        /// <summary>
        /// 根据页面名称获取已发布页面（前端渲染用）
        /// </summary>
        /// <param name="pageName">页面名称</param>
        /// <returns>页面详情</returns>
        [HttpGet("render/{pageName}")]
        public async Task<IActionResult> GetPublishedPage(string pageName)
        {
            try
            {
                var page = await _pageService.GetPublishedPageByNameAsync(pageName);
                if (page == null)
                {
                    return NotFound(new { Code = 404, Msg = "页面不存在或未发布" });
                }
                return Ok(new { Code = 200, Msg = "获取成功", Data = page });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Code = 400, Msg = ex.Message });
            }
        }

        /// <summary>
        /// 创建新页面
        /// </summary>
        /// <param name="entity">页面配置实体</param>
        /// <returns>页面ID</returns>
        [HttpPost("create")]
        public async Task<IActionResult> CreatePage([FromBody] PageEntity entity)
        {
            try
            {
                var pageId = await _pageService.CreatePageAsync(entity);
                return Ok(new { Code = 200, Msg = "创建成功", Data = pageId });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Code = 400, Msg = ex.Message });
            }
        }

        /// <summary>
        /// 更新页面配置
        /// </summary>
        /// <param name="entity">页面配置实体</param>
        /// <returns>更新结果</returns>
        [HttpPut("update")]
        public async Task<IActionResult> UpdatePage([FromBody] PageEntity entity)
        {
            try
            {
                var result = await _pageService.UpdatePageAsync(entity);
                return Ok(new { Code = 200, Msg = "更新成功", Data = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Code = 400, Msg = ex.Message });
            }
        }

        /// <summary>
        /// 删除页面
        /// </summary>
        /// <param name="id">页面唯一ID</param>
        /// <returns>删除结果</returns>
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeletePage(Guid id)
        {
            try
            {
                var result = await _pageService.DeletePageAsync(id);
                return Ok(new { Code = 200, Msg = "删除成功", Data = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Code = 400, Msg = ex.Message });
            }
        }

        /// <summary>
        /// 发布/取消发布页面
        /// </summary>
        /// <param name="id">页面唯一ID</param>
        /// <param name="publishStatus">发布状态：0-草稿 1-已发布</param>
        /// <returns>操作结果</returns>
        [HttpPut("publish/{id}")]
        public async Task<IActionResult> PublishPage(Guid id, [FromQuery] int publishStatus)
        {
            try
            {
                var result = await _pageService.PublishPageAsync(id, publishStatus);
                var msg = publishStatus == 1 ? "发布成功" : "取消发布成功";
                return Ok(new { Code = 200, Msg = msg, Data = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Code = 400, Msg = ex.Message });
            }
        }
    }
}
