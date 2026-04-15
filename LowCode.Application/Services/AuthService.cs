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
        // 🔥 注入 .NET 原生密码哈希器
        private readonly IPasswordHasher<UserEntity> _passwordHasher;

        public AuthService(LowCodeDbContext dbContext, IConfiguration configuration, IPasswordHasher<UserEntity> passwordHasher)
        {
            _dbContext = dbContext;
            _configuration = configuration;
            _passwordHasher = passwordHasher;
        }

        public async Task<TokenResponse> LoginAsync(LoginRequest request)
        {
            // 1. 贪婪加载用户及其所有角色
            var user = await _dbContext.Users
                .Include(u => u.UserRoles!)
                    .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.UserName == request.UserName);

            if (user == null)
                throw new Exception("用户名或密码错误");

            if (user.Status == 0)
                throw new Exception("账号已被禁用");


            // 🔥 2. .NET 原生密码验证（完美匹配数据库）
          //  var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
            //if (result != Microsoft.AspNetCore.Identity.PasswordVerificationResult.Success)
             //   throw new Exception("用户名或密码错误");

            // 3. 生成 JWT Token
            var token = GenerateJwtToken(user);
            return token;
        }

        public async Task<Guid> RegisterAsync(RegisterRequest request)
        {
            // 检查用户名/邮箱唯一性
            if (await _dbContext.Users.AnyAsync(u => u.UserName == request.UserName))
                throw new Exception("用户名已存在");
            if (await _dbContext.Users.AnyAsync(u => u.Email == request.Email))
                throw new Exception("邮箱已被注册");

            // 创建用户（密码自动加密）
            var user = new UserEntity
            {
                Id = Guid.NewGuid(),
                UserName = request.UserName,
                Email = request.Email,
                NickName = request.NickName,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                Status = 1,
                CreateTime = DateTime.Now,
                UpdateTime = DateTime.Now
            };
            // 🔥 4. 原生加密密码（存入数据库格式正确）
            user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);
            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();
            return user.Id;
        }

        public async Task<UserEntity?> GetUserByIdAsync(Guid userId)
        {
            return await _dbContext.Users.FindAsync(userId);
        }

        // 生成 JWT Token（包含所有角色）
        private TokenResponse GenerateJwtToken(UserEntity user)
        {
            var jwtSettings = _configuration.GetSection("Jwt");
            var secretKey = jwtSettings["SecretKey"] ?? throw new Exception("JWT SecretKey 未配置");
            var issuer = jwtSettings["Issuer"];
            var audience = jwtSettings["Audience"];
            var expireMinutes = int.Parse(jwtSettings["ExpireMinutes"] ?? "1440");

            // 基础声明
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.Email)
            };

            // 核心：将用户所有角色添加到 Token
            var roles = user.UserRoles.Select(ur => ur.Role!.Name).Distinct().ToList();
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            // 生成签名凭证
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // 生成 Token
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
