using LowCode.Application.Interfaces;
using LowCode.Domain.Entities;
using LowCode.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace LowCode.Web.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FormController : ControllerBase
    {
        /// <summary>
        /// 注入表单服务
        /// </summary>
        private readonly IFormService _formService;
        public FormController(IFormService formService)
        {
            _formService = formService;
        }

        #region 表单基础CRUD接口
        /// <summary>
        /// 获取表单列表
        /// </summary>
        [HttpGet("list")]
        public async Task<ApiResult<List<FormEntity>>> GetFormList()
        {
            return await _formService.GetFormListAsync();
        }

        /// <summary>
        /// 根据ID查询表单
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ApiResult<FormEntity?>> GetFormById(Guid id)
        {
            return await _formService.GetFormByIdAsync(id);
        }

        /// <summary>
        /// 根据编码查询表单
        /// </summary>
        [HttpGet("code/{formCode}")]
        public async Task<ApiResult<FormEntity?>> GetFormByCode(string formCode)
        {
            return await _formService.GetFormByCodeAsync(formCode);
        }

        /// <summary>
        /// 创建表单
        /// </summary>
        [HttpPost("create")]
        public async Task<ApiResult<Guid>> CreateForm([FromBody] FormEntity entity)
        {
            return await _formService.CreateFormAsync(entity);
        }

        /// <summary>
        /// 更新表单
        /// </summary>
        [HttpPut("update")]
        public async Task<ApiResult<bool>> UpdateForm([FromBody] FormEntity entity)
        {
            return await _formService.UpdateFormAsync(entity);
        }

        /// <summary>
        /// 删除表单
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ApiResult<bool>> DeleteForm(Guid id)
        {
            return await _formService.DeleteFormAsync(id);
        }
        #endregion

        #region 低代码核心接口
        /// <summary>
        /// 发布表单
        /// </summary>
        [HttpPost("publish/{id}")]
        public async Task<ApiResult<string>> PublishForm(Guid id)
        {
            return await _formService.PublishFormAsync(id);
        }

        /// <summary>
        /// 获取表单渲染配置（前端低代码画布核心接口）
        /// </summary>
        [HttpGet("render/{formId}")]
        public async Task<ApiResult<object>> GetFormRenderConfig(Guid formId)
        {
            return await _formService.GetFormRenderConfigAsync(formId);
        }

        /// <summary>
        /// 获取表单绑定的数据源数据
        /// </summary>
        [HttpGet("datasource/{formId}")]
        public async Task<ApiResult<Object>> GetFormDataSourceData(Guid formId)
        {
            return await _formService.GetFormDataSourceDataAsync(formId);
        }
        #endregion
    }
}