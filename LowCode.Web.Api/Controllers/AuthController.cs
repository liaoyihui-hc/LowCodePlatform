﻿using LowCode.Application.Interfaces;
using LowCode.Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LowCode.Web.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService) => _authService = authService;

        [HttpPost("login")]
        public async Task<ApiResult<TokenResponse>> Login([FromBody] LoginRequest request)
        {
            var res = await _authService.LoginAsync(request);
            return ApiResult<TokenResponse>.Success(res.Data);
        }

        [HttpPost("register")]
        public async Task<ApiResult<Guid>> Register([FromBody] RegisterRequest request)
        {
            var id = await _authService.RegisterAsync(request);
            return ApiResult<Guid>.Success(id.Data);
        }

    }
}
