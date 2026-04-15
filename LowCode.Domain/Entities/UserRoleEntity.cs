using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace LowCode.Domain.Entities
{
   public class UserRoleEntity : IdentityUserRole<Guid>
    {

        [Key]
        public Guid Id { get; set; }

  
        public UserEntity? User { get; set; }

    
        public RoleEntity? Role { get; set; }
    }
}
