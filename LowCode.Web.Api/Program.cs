using LowCode.Application.Interfaces;
using LowCode.Application.Services;
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

// Swagger 配置（修复冲突，保留标准用法）
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "LowCode API", Version = "v1" });
});

// 数据库上下文 + Identity
builder.Services.AddDbContext<LowCodeDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("LowCodeDb")));

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

// JWT 认证
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

// 跨域配置（修改为你前端的 3002 端口）
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowVueFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:3002")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// 缓存服务注册
builder.Services.AddMemoryCache();

// 业务服务注册
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IPageService, PageService>();
builder.Services.AddScoped<IComponentMetaService, ComponentMetaService>();
builder.Services.AddScoped<IFormService, FormService>();

var app = builder.Build();

// 1. 全局异常中间件（最优先）
app.UseMiddleware<GlobalExceptionMiddleware>();

// 2. 模型验证中间件
app.UseMiddleware<ModelValidationMiddleware>();

// 3. 开发环境 Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "LowCode API V1"));
}

// ------------------------------
// ✅ 官方固定中间件顺序（核心！）
// ------------------------------
// 3. HTTPS重定向
app.UseHttpsRedirection();
// 4. 路由
app.UseRouting();
// 5. 跨域（必须在 认证/授权 之前！）
app.UseCors("AllowVueFrontend");
// 6. 认证 → 7. 授权
app.UseAuthentication();
app.UseAuthorization();

// 映射控制器
app.MapControllers();

// 初始化数据
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