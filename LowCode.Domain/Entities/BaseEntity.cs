using System;
using System.Collections.Generic;
using System.Text;

namespace LowCode.Domain.Entities
{
    public class BaseEntity
    {

        
            /// <summary>
            /// 主键ID
            /// </summary>
            public Guid Id { get; set; }

            /// <summary>
            /// 创建时间
            /// </summary>
            public DateTime CreateTime { get; set; } = DateTime.Now;

            /// <summary>
            /// 更新时间
            /// </summary>
            public DateTime? UpdateTime { get; set; }

            /// <summary>
            /// 软删除标识
            /// </summary>
            public bool IsDeleted { get; set; } = false;
     }
    
}
