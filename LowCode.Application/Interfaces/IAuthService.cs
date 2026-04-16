﻿using LowCode.Domain.Entities;
using LowCode.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace LowCode.Application.Interfaces
{
    public interface IAuthService
    {
        Task<ApiResult<TokenResponse>> LoginAsync(LoginRequest request);
        Task<ApiResult<Guid>> RegisterAsync(RegisterRequest request);
        Task<ApiResult<UserEntity?>> GetUserByIdAsync(Guid userId);
    }
}
