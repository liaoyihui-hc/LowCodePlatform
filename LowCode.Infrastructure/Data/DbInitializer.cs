using LowCode.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace LowCode.Infrastructure.Data
{
    public static class DbInitializer
    {
        public static async Task SeedData(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<UserEntity>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<RoleEntity>>();

            var fixedTime = new DateTime(2026, 4, 15, 0, 0, 0, DateTimeKind.Utc);

            // 1. 创建 Admin 角色
            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                await roleManager.CreateAsync(new RoleEntity
                {
                    Name = "Admin", // 注意：Identity 基类用的是 Name，不是 RoleName
                    Description = "系统管理员",
                    CreateTime = fixedTime
                });
            }

            // 2. 创建 Admin 用户
            var adminUser = await userManager.FindByNameAsync("admin");
            if (adminUser == null)
            {
                adminUser = new UserEntity
                {
                    UserName = "admin",
                    Email = "admin@lowcode.com",
                    NickName = "系统管理员",
                    Status = 1,
                    CreateTime = fixedTime,
                    UpdateTime = fixedTime,
                    EmailConfirmed = true // 必须确认，否则可能无法登录
                };

                // 👉 这里传入明文密码，UserManager 会自动加密成正确的 Hash
                var result = await userManager.CreateAsync(adminUser, "Admin@123");

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }
        }
    }
}
