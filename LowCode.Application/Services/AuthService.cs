using BCrypt.Net;
using LowCode.Application.Interfaces;
using LowCode.Domain.Entities;
using LowCode.Domain.Models;
using LowCode.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace LowCode.Application.Services
{
    public class AuthService:IAuthService
    {

        private readonly LowCodeDbContext _dbContext;
        private readonly IConfiguration _configuration;
        private readonly IPasswordHasher<UserEntity> _passwordHasher;

        public AuthService(LowCodeDbContext dbContext, IConfiguration configuration, IPasswordHasher<UserEntity> passwordHasher)
        {
            _dbContext = dbContext;
            _configuration = configuration;
            _passwordHasher = passwordHasher;
        }

        public async Task<ApiResult<TokenResponse>> LoginAsync(LoginRequest request)
        {
            var user = await _dbContext.Users
                .Include(u => u.UserRoles!)
                    .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.UserName == request.UserName);

            if (user == null)
                throw new Exception("用户名或密码错误");

            if (user.Status == 0)
                throw new Exception("账号已被禁用");

            var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
            if (result != Microsoft.AspNetCore.Identity.PasswordVerificationResult.Success)
                throw new Exception("用户名或密码错误");

            var token = GenerateJwtToken(user);
            return ApiResult<TokenResponse>.Success(token);
        }

        public async Task<ApiResult<Guid>> RegisterAsync(RegisterRequest request)
        {
            if (await _dbContext.Users.AnyAsync(u => u.UserName == request.UserName))
                throw new Exception("用户名已存在");
            if (await _dbContext.Users.AnyAsync(u => u.Email == request.Email))
                throw new Exception("邮箱已被注册");

            var user = new UserEntity
            {
                Id = Guid.NewGuid(),
                UserName = request.UserName,
                Email = request.Email,
                NickName = request.NickName,
                Status = 1,
                CreateTime = DateTime.Now,
                UpdateTime = DateTime.Now
            };
            user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);
            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();
            return ApiResult<Guid>.Success(user.Id);
        }

        public async Task<ApiResult<UserEntity?>> GetUserByIdAsync(Guid userId)
        {
            var user = await _dbContext.Users.FindAsync(userId);
            return ApiResult<UserEntity?>.Success(user);
        }

        private TokenResponse GenerateJwtToken(UserEntity user)
        {
            var jwtSettings = _configuration.GetSection("Jwt");
            var secretKey = jwtSettings["SecretKey"] ?? throw new Exception("JWT SecretKey 未配置");
            var issuer = jwtSettings["Issuer"];
            var audience = jwtSettings["Audience"];
            var expireMinutes = int.Parse(jwtSettings["ExpireMinutes"] ?? "1440");

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.Email)
            };

            var roles = user.UserRoles.Select(ur => ur.Role!.Name).Distinct().ToList();
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(expireMinutes),
                signingCredentials: creds);

            return new TokenResponse
            {
                AccessToken = new JwtSecurityTokenHandler().WriteToken(token),
                ExpiresIn = expireMinutes * 60
            };
        }
    }
}
