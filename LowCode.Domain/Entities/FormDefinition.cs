using System;
using System.Collections.Generic;
using System.Text;

namespace LowCode.Domain.Entities
{
    public  class FormDefinition : BaseEntity
    {
        /// <summary>
        /// 表单名称
        /// </summary>
        public string FormName { get; set; }

        /// <summary>
        /// 表单唯一标识
        /// </summary>
        public string FormCode { get; set; }

        /// <summary>
        /// 前端设计器JSON配置
        /// </summary>
        public string FormConfig { get; set; }

        /// <summary>
        /// 表单状态
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 备注描述
        /// </summary>
        public string? Description { get; set; }
    }
    
}
