using LowCode.Application.Interfaces;
using LowCode.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LowCode.Web.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ComponentMetasController : ControllerBase
    {
        private readonly IComponentMetaService _componentService;

        public ComponentMetasController(IComponentMetaService componentService)
        {
            _componentService = componentService;
        }

        [HttpGet("list")]
        public async Task<IActionResult> GetComponentList()
        {
            try
            {
                var list = await _componentService.GetComponentListAsync();
                return Ok(new { Code = 200, Msg = "获取成功", Data = list });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Code = 400, Msg = ex.Message });
            }
        }

        [HttpGet("detail/{id}")]
        public async Task<IActionResult> GetComponentById(Guid id)
        {
            try
            {
                var component = await _componentService.GetComponentByIdAsync(id);
                return component == null ? NotFound(new { Code = 404, Msg = "组件不存在" }) : Ok(new { Code = 200, Msg = "获取成功", Data = component });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Code = 400, Msg = ex.Message });
            }
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateComponent([FromBody] ComponentMetaEntity entity)
        {
            try
            {
                var id = await _componentService.CreateComponentAsync(entity);
                return Ok(new { Code = 200, Msg = "创建成功", Data = id });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Code = 400, Msg = ex.Message });
            }
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateComponent([FromBody] ComponentMetaEntity entity)
        {
            try
            {
                var result = await _componentService.UpdateComponentAsync(entity);
                return Ok(new { Code = 200, Msg = "更新成功", Data = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Code = 400, Msg = ex.Message });
            }
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteComponent(Guid id)
        {
            try
            {
                var result = await _componentService.DeleteComponentAsync(id);
                return Ok(new { Code = 200, Msg = "删除成功", Data = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Code = 400, Msg = ex.Message });
            }
        }

        [HttpPut("toggle/{id}")]
        public async Task<IActionResult> ToggleComponentStatus(Guid id, [FromQuery] int isEnable)
        {
            try
            {
                var result = await _componentService.ToggleComponentStatusAsync(id, isEnable);
                var msg = isEnable == 1 ? "启用成功" : "禁用成功";
                return Ok(new { Code = 200, Msg = msg, Data = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Code = 400, Msg = ex.Message });
            }
        }
    }
}
