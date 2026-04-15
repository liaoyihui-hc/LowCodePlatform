using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace LowCode.Domain.Models
{

    /// <summary>
    /// 登录请求模型：登录接口入参契约
    /// </summary>
    public class LoginRequest
    {
        /// <summary>
        /// 登录用户名
        /// </summary>
        [Required(ErrorMessage = "用户名不能为空")]
        public string UserName { get; set; } = string.Empty;

        /// <summary>
        /// 登录密码（明文传输，生产环境必须启用HTTPS）
        /// </summary>
        [Required(ErrorMessage = "密码不能为空")]
        public string Password { get; set; } = string.Empty;
    }

    /// <summary>
    /// 注册请求模型：注册接口入参契约
    /// </summary>
    public class RegisterRequest
    {
        /// <summary>
        /// 用户名（全局唯一）
        /// </summary>
        [Required(ErrorMessage = "用户名不能为空")]
        [MaxLength(50, ErrorMessage = "用户名最长50个字符")]
        public string UserName { get; set; } = string.Empty;

        /// <summary>
        /// 登录密码
        /// </summary>
        [Required(ErrorMessage = "密码不能为空")]
        [MinLength(6, ErrorMessage = "密码最少6位")]
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// 邮箱（全局唯一，用于找回密码等操作）
        /// </summary>
        [Required(ErrorMessage = "邮箱不能为空")]
        [EmailAddress(ErrorMessage = "邮箱格式不正确")]
        [MaxLength(100, ErrorMessage = "邮箱最长100个字符")]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// 用户昵称（可选）
        /// </summary>
        [MaxLength(50, ErrorMessage = "昵称最长50个字符")]
        public string? NickName { get; set; }
    }

    /// <summary>
    /// Token响应模型：登录成功后返回的令牌契约
    /// 符合 OAuth 2.0 Bearer Token 规范（RFC 6750）
    /// </summary>
    public class TokenResponse
    {
        /// <summary>
        /// 访问令牌（核心：JWT格式的身份凭证）
        /// </summary>
        public string AccessToken { get; set; } = string.Empty;

        /// <summary>
        /// 令牌类型（固定值：Bearer）
        /// </summary>
        public string TokenType { get; set; } = "Bearer";

        /// <summary>
        /// 令牌有效期（单位：秒）
        /// </summary>
        public int ExpiresIn { get; set; }
    }
}
