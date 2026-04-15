using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace LowCode.Domain.Entities
{
    public class RoleEntity : IdentityRole<Guid>
    {
      

        [MaxLength(200)]
        public string? Description { get; set; }

        public DateTime CreateTime { get; set; } = DateTime.Now;

        // 导航属性：角色-用户多对多
        public ICollection<UserRoleEntity> UserRoles { get; set; } = new List<UserRoleEntity>();
    }
}
