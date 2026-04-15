using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace LowCode.Domain.Entities
{
    public class ComponentMetaEntity
    {
        /// <summary>
        /// 组件唯一ID
        /// </summary>
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// 组件类型（唯一标识，比如el-input、el-button）
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string ComponentType { get; set; } = string.Empty;

        /// <summary>
        /// 组件名称（中文显示，比如单行输入框、主按钮）
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string ComponentName { get; set; } = string.Empty;

        /// <summary>
        /// 组件分组（比如基础组件、表单组件、布局组件）
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string GroupName { get; set; } = string.Empty;

        /// <summary>
        /// 组件默认属性JSON（比如默认宽度、占位符、按钮类型）
        /// </summary>
        [Required]
        public string DefaultPropsJson { get; set; } = "{}";

        /// <summary>
        /// 组件图标（前端显示用，图标类名/地址）
        /// </summary>
        [MaxLength(100)]
        public string? Icon { get; set; }

        /// <summary>
        /// 是否启用：0-禁用 1-启用
        /// </summary>
        public int IsEnable { get; set; } = 1;

        /// <summary>
        /// 排序号（越小越靠前）
        /// </summary>
        public int Sort { get; set; } = 0;
    }
}
