using LowCode.Application.Interfaces;
using LowCode.Domain.Entities;
using LowCode.Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LowCode.Web.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class FormsController : ControllerBase
    {
        private readonly IFormService _formService;

        public FormsController(IFormService formService)
        {
            _formService = formService;
        }

        [HttpGet("list")]
        public async Task<IActionResult> GetFormList()
        {
            var list = await _formService.GetFormListAsync();
            return Ok(ApiResponse<List<FormEntity>>.Success(list, "获取成功"));
        }

        [HttpGet("detail/{id}")]
        public async Task<IActionResult> GetFormById(Guid id)
        {
            var form = await _formService.GetFormByIdAsync(id);
            if (form == null) return NotFound(ApiResponse<FormEntity>.Fail("表单不存在", 404));
            return Ok(ApiResponse<FormEntity>.Success(form, "获取成功"));
        }

        [HttpGet("code/{formCode}")]
        public async Task<IActionResult> GetFormByCode(string formCode)
        {
            var form = await _formService.GetFormByCodeAsync(formCode);
            if (form == null) return NotFound(ApiResponse<FormEntity>.Fail("表单不存在", 404));
            return Ok(ApiResponse<FormEntity>.Success(form, "获取成功"));
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateForm([FromBody] FormEntity entity)
        {
            var id = await _formService.CreateFormAsync(entity);
            return Ok(ApiResponse<Guid>.Success(id, "创建成功"));
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateForm([FromBody] FormEntity entity)
        {
            var result = await _formService.UpdateFormAsync(entity);
            return Ok(ApiResponse<bool>.Success(result, "更新成功"));
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteForm(Guid id)
        {
            var result = await _formService.DeleteFormAsync(id);
            return Ok(ApiResponse<bool>.Success(result, "删除成功"));
        }
    }
}
