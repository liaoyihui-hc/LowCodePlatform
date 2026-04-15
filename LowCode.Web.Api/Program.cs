using LowCode.Application.Interfaces;
using LowCode.Application.Services;    // 引用实现类 👈 关键是这个！
using LowCode.Domain.Entities;
using LowCode.Infrastructure;
using LowCode.Infrastructure.Data;
using LowCode.Web.Api.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using System.Text;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// 2. 添加Swagger接口文档服务（VS自带，开发环境用）
builder.Services.AddEndpointsApiExplorer();


builder.Services.AddSwaggerGen(); // 就这一行，不报错！

// 1. 数据库上下文 + Identity 核心配置（必须改这里！）
builder.Services.AddDbContext<LowCodeDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("LowCodeDb")));
// 👇 关键修复：注册 ASP.NET Core Identity（你之前漏了这个，导致密码加密/验证失败）
builder.Services.AddIdentity<UserEntity, RoleEntity>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 6;
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<LowCodeDbContext>()
.AddDefaultTokenProviders();
// 4. JWT 认证配置
var jwtSection = builder.Configuration.GetSection("Jwt");
var secretKey = Encoding.UTF8.GetBytes(jwtSection["SecretKey"]!);
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSection["Issuer"],
        ValidAudience = jwtSection["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(secretKey),
        ClockSkew = TimeSpan.Zero
    };
});

// 5. 配置跨域
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowVueFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});
// 6. 注册业务服务

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IPageService, PageService>();
builder.Services.AddScoped<IComponentMetaService, ComponentMetaService>(); // 👈 新增这行
builder.Services.AddScoped<IFormService, FormService>();
var app = builder.Build();
// 全局异常处理中间件（必须放在最前面！）
app.UseMiddleware<GlobalExceptionMiddleware>();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI(); // 访问地址：http://localhost:xxxx/swagger
}
// 启用跨域（必须放在路由之后，控制器之前，顺序不能错）
app.UseCors("AllowVueFrontend");

// 启用路由
app.UseRouting();
app.UseHttpsRedirection();
// 核心：认证必须在授权之前
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
// 👇 最后：自动初始化管理员账号（密码自动加密）
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        await DbInitializer.SeedData(services);
        Console.WriteLine("✅ 超级管理员初始化完成：admin / Admin@123");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ 初始化失败：{ex.Message}");
    }
}
app.Run();
