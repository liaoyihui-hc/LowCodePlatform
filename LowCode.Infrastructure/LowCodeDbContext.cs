using LowCode.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace LowCode.Infrastructure
{

    // 👇 关键修改 1：继承 IdentityDbContext，指定实体和主键类型
    public class LowCodeDbContext : IdentityDbContext<UserEntity, RoleEntity, Guid,
        IdentityUserClaim<Guid>, UserRoleEntity, IdentityUserLogin<Guid>,
        IdentityRoleClaim<Guid>, IdentityUserToken<Guid>>
    {
        public LowCodeDbContext(DbContextOptions<LowCodeDbContext> options) : base(options)
        {
        }
        // 低代码表单表（唯一新增行，其他都是你原有代码）
        public DbSet<FormDefinition> FormDefinitions { get; set; }
        // 业务表映射（保留）
        public DbSet<PageEntity> Pages => Set<PageEntity>();
        public DbSet<ComponentMetaEntity> ComponentMetas => Set<ComponentMetaEntity>();
        public DbSet<FormEntity> Forms => Set<FormEntity>();

        // 👇 关键修改 2：Identity 相关的 DbSet 可以移除（基类已包含），
        // 但如果你想显式保留也可以，这里为了简洁建议移除。
        // public DbSet<UserEntity> Users => Set<UserEntity>();
        // public DbSet<RoleEntity> Roles => Set<RoleEntity>();
        // public DbSet<UserRoleEntity> UserRoles => Set<UserRoleEntity>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // 👇 必须先调用基类方法，配置 Identity 自带的所有表结构
            base.OnModelCreating(modelBuilder);

            // 原有业务表索引配置（保留）
            modelBuilder.Entity<PageEntity>().HasIndex(p => p.PageName).IsUnique();
            modelBuilder.Entity<ComponentMetaEntity>().HasIndex(c => c.ComponentType).IsUnique();
            modelBuilder.Entity<FormEntity>().HasIndex(f => f.FormCode).IsUnique();

            // 👇 关键修改 3：简化用户-角色多对多配置（基类已处理 UserId/RoleId）
            // 只需配置导航属性即可
            modelBuilder.Entity<UserRoleEntity>(b =>
            {
                // 配置与 User 的关系
                b.HasOne(ur => ur.User)
                    .WithMany(u => u.UserRoles)
                    .HasForeignKey(ur => ur.UserId);

                // 配置与 Role 的关系
                b.HasOne(ur => ur.Role)
                    .WithMany(r => r.UserRoles)
                    .HasForeignKey(ur => ur.RoleId);
            });

            // 👇 关键修改 4：彻底移除 modelBuilder.Entity<...>().HasData(...) 
            // (用户、角色、UserRole 的种子数据全部删除，改用 DbInitializer 初始化)

            // 原有组件种子数据（保留，因为不涉及密码）
            modelBuilder.Entity<ComponentMetaEntity>().HasData(
                new ComponentMetaEntity
                {
                    Id = Guid.Parse("550e8400-e29b-41d4-a716-446655440000"),
                    ComponentType = "el-button",
                    ComponentName = "按钮",
                    GroupName = "基础组件",
                    DefaultPropsJson = "{\"type\":\"primary\",\"size\":\"default\",\"content\":\"按钮\"}",
                    Icon = "icon-button",
                    IsEnable = 1,
                    Sort = 1
                },
                new ComponentMetaEntity
                {
                    Id = Guid.Parse("550e8400-e29b-41d4-a716-446655440001"),
                    ComponentType = "el-input",
                    ComponentName = "输入框",
                    GroupName = "基础组件",
                    DefaultPropsJson = "{\"placeholder\":\"请输入\",\"size\":\"default\"}",
                    Icon = "icon-input",
                    IsEnable = 1,
                    Sort = 2
                }
            );


        }

        // 🔥 修正：仅修改时更新 UpdateTime，新增由实体默认值处理
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            // 只处理【修改操作】的更新时间
            foreach (var item in ChangeTracker.Entries<BaseEntity>().Where(x => x.State == EntityState.Modified))
            {
                item.Entity.UpdateTime = DateTime.Now;
            }

            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
