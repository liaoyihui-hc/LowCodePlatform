﻿using LowCode.Application.Interfaces;
using LowCode.Domain.Entities;
using LowCode.Domain.Models;
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
        public async Task<ApiResult<List<ComponentMetaEntity>>> GetComponentList()
        {
            var list = await _componentService.GetComponentListAsync();
            return ApiResult<List<ComponentMetaEntity>>.Success(list);
        }

        [HttpGet("detail/{id}")]
        public async Task<ApiResult<ComponentMetaEntity?>> GetComponentById(Guid id)
        {
            var component = await _componentService.GetComponentByIdAsync(id);
            return ApiResult<ComponentMetaEntity?>.Success(component);
        }

        [HttpPost("create")]
        public async Task<ApiResult<Guid>> CreateComponent([FromBody] ComponentMetaEntity entity)
        {
            var id = await _componentService.CreateComponentAsync(entity);
            return ApiResult<Guid>.Success(id);
        }

        [HttpPut("update")]
        public async Task<ApiResult<bool>> UpdateComponent([FromBody] ComponentMetaEntity entity)
        {
            var result = await _componentService.UpdateComponentAsync(entity);
            return ApiResult<bool>.Success(result);
        }

        [HttpDelete("delete/{id}")]
        public async Task<ApiResult<bool>> DeleteComponent(Guid id)
        {
            var result = await _componentService.DeleteComponentAsync(id);
            return ApiResult<bool>.Success(result);
        }

        [HttpPut("toggle/{id}")]
        public async Task<ApiResult<bool>> ToggleComponentStatus(Guid id, [FromQuery] int isEnable)
        {
            var result = await _componentService.ToggleComponentStatusAsync(id, isEnable);
            return ApiResult<bool>.Success(result);
        }
    }
}
