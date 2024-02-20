using System.Security.Claims;
using System.Text;
using AppProject.Common;
using AppProject.Common.Constants;
using AppProject.Common.Option;
using AppProject.Extensions.ServiceExtensions.Authorizations.Policys;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace AppProject.Extensions.ServiceExtensions.Authorizations;

public static class AuthorizationSetup
{
    /// <summary>
    /// 系统授权服务配置
    /// </summary>
    /// <param name="services"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public static void AddAuthorizationSetup(this IServiceCollection services)
    {
        if (services is null) throw new ArgumentNullException(nameof(services));

        services.AddAuthorization(options =>
        {
            options.AddPolicy(PolicyConstant.CLIENT, policy => policy.RequireRole(RoleConstant.CLIENT).Build());
            options.AddPolicy(PolicyConstant.ADMIN, policy => policy.RequireRole(RoleConstant.ADMIN).Build());
            options.AddPolicy(PolicyConstant.SYSTEM_OR_ADMIN,
                policy => policy.RequireRole(RoleConstant.ADMIN, RoleConstant.SYETEM));
            options.AddPolicy(PolicyConstant.A_S_O,
                policy => policy.RequireRole(RoleConstant.ADMIN, RoleConstant.SYETEM, RoleConstant.OTHER));
        });

        //读取配置参数
        var jwtSettings = App.GetOptionsMonitor<JwtSettingsOptions>()??throw new ArgumentNullException(nameof(JwtSettingsOptions));
        var secretKey = jwtSettings.SecretKey;
        var keyByteArray = Encoding.UTF8.GetBytes(secretKey);
        var signingKey = new SymmetricSecurityKey(keyByteArray);
        var issuer = jwtSettings.ValidIssuer;
        var audience = jwtSettings.ValidAudience;

        var signingCredentials = new SigningCredentials(signingKey,SecurityAlgorithms.HmacSha256);

        var permissions = new List<PermissionItem>();

        var permissionRequirement = new PermissionRequirement(
            permissions,
            "",
            ClaimTypes.Role,
            issuer,
            audience,
            TimeSpan.FromMinutes(jwtSettings.Expires),
            signingCredentials
        );


        services.AddAuthorization(option =>
        {
            option.AddPolicy(Permissions.Name,
                policy => policy.Requirements.Add(permissionRequirement));
        });


        services.AddSingleton<IAuthorizationHandler, PermissionHandler>();
        services.AddSingleton(permissionRequirement);
    }
}