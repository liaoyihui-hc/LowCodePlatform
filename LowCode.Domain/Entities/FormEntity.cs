using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace LowCode.Domain.Entities
{
    public class FormEntity
    {/// <summary>
     /// 表单唯一ID
     /// </summary>
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// 表单名称
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string FormName { get; set; } = string.Empty;

        /// <summary>
        /// 表单标识（唯一，用于接口调用）
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string FormCode { get; set; } = string.Empty;

        /// <summary>
        /// 表单字段配置JSON（字段、类型、校验规则、默认值）
        /// </summary>
        [Required]
        public string FieldsConfigJson { get; set; } = "[]";

        /// <summary>
        /// 表单提交后回调接口地址
        /// </summary>
        [MaxLength(500)]
        public string? SubmitApiUrl { get; set; }

        /// <summary>
        /// 是否启用数据存储：0-不存储 1-存储（自动生成表存储表单提交数据）
        /// </summary>
        public int IsSaveData { get; set; } = 0;

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
