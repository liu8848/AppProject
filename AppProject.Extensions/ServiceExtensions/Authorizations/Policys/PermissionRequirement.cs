using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;

namespace AppProject.Extensions.ServiceExtensions.Authorizations.Policys;

public class PermissionRequirement : IAuthorizationRequirement
{
    public List<PermissionItem> Permissions { get; set; }

    /// <summary>
    /// 无权限action
    /// </summary>
    public string DeniedAction { get; set; }

    /// <summary>
    /// 认证授权类型
    /// </summary>
    public string ClaimType { internal get; set; }

    /// <summary>
    /// 请求路径
    /// </summary>
    public string LoginPath { get; set; } = "/api/Login";

    /// <summary>
    /// 发行人
    /// </summary>
    public string Issuer { get; set; }

    /// <summary>
    /// 订阅人
    /// </summary>
    public string Audience { get; set; }

    /// <summary>
    /// 过期时间
    /// </summary>
    public TimeSpan Expiration { get; set; }

    /// <summary>
    /// 签名验证
    /// </summary>
    public SigningCredentials SigningCredentials { get; set; }

    public PermissionRequirement(List<PermissionItem> permissions, string deniedAction, string claimType,
        string issuer, string audience, TimeSpan expiration, SigningCredentials signingCredentials)
    {
        Permissions = permissions;
        DeniedAction = deniedAction;
        ClaimType = claimType;
        Issuer = issuer;
        Audience = audience;
        Expiration = expiration;
        SigningCredentials = signingCredentials;
    }
}