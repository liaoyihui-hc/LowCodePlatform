using System;
using System.Collections.Generic;
using System.Text;

namespace LowCode.Domain.Entities
{
    public class PageConfig
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string PageName { get; set; } = string.Empty; // 页面名称
        public string PageJson { get; set; } = string.Empty; // 核心：前端拖拽的组件配置JSON
        public DateTime CreateTime { get; set; } = DateTime.Now;

    }
}
