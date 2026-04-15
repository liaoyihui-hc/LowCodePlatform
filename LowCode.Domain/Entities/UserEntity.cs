using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace LowCode.Domain.Entities
{
    public class UserEntity : IdentityUser<Guid>
    {

    

        [MaxLength(50)]
        public string? NickName { get; set; }

        public int Status { get; set; } = 1; // 1: 正常, 0: 禁用
        public DateTime CreateTime { get; set; } = DateTime.Now;
        public DateTime UpdateTime { get; set; } = DateTime.Now;

        // 导航属性：用户-角色多对多
        public ICollection<UserRoleEntity> UserRoles { get; set; } = new List<UserRoleEntity>();
    }
}
