using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace LowCode.Domain.Entities
{
    public class PageEntity
    {
        /// <summary>
        /// 页面唯一ID（主键）
        /// </summary>
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// 页面名称（唯一，用于前端路由）
        /// </summary>
        [Required(ErrorMessage = "页面名称不能为空")]
        [MaxLength(100, ErrorMessage = "页面名称最多100个字符")]
        public string PageName { get; set; } = string.Empty;

        /// <summary>
        /// 页面标题（用于显示）
        /// </summary>
        [MaxLength(200, ErrorMessage = "页面标题最多200个字符")]
        public string? PageTitle { get; set; }

        /// <summary>
        /// 页面组件树JSON（前端拖拽生成的核心元数据）
        /// </summary>
        [Required(ErrorMessage = "页面组件配置不能为空")]
        public string ComponentTreeJson { get; set; } = "[]";

        /// <summary>
        /// 页面发布状态：0-草稿 1-已发布
        /// </summary>
        public int PublishStatus { get; set; } = 0;

        /// <summary>
        /// 创建人ID
        /// </summary>
        public Guid CreateUserId { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; } = DateTime.Now;

        /// <summary>
        /// 最后更新时间
        /// </summary>
        public DateTime UpdateTime { get; set; } = DateTime.Now;
    }
}
