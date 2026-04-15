using LowCode.Domain.Entities;
using LowCode.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace LowCode.Application.Interfaces
{
    public interface IAuthService
    {
        Task<TokenResponse> LoginAsync(LoginRequest request);
        Task<Guid> RegisterAsync(RegisterRequest request);
        Task<UserEntity?> GetUserByIdAsync(Guid userId);
    }
}
