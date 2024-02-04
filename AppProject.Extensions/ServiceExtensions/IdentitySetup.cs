using System.Text;
using AppProject.Common;
using AppProject.Common.Option;
using AppProject.IService.Identities;
using AppProject.Model.Entities.Identities;
using AppProject.Repository.Context;
using AppProject.Services.Identities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace AppProject.Extensions.ServiceExtensions;

public static class IdentitySetup
{
    public static IServiceCollection AddIdentityExtension(this IServiceCollection services)
    {
        services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
            {
                //密码设置要求
                options.Password.RequireDigit = true; //数字必须
                options.Password.RequiredLength = 6; //密码长度大于6
                options.Password.RequireLowercase = false; //小写字母非必须
                options.Password.RequireUppercase = false; //大写字母非必须
                options.Password.RequireNonAlphanumeric = false; //特殊字符非必须
            })
            .AddEntityFrameworkStores<AppProjectDbContext>()
            .AddDefaultTokenProviders()
            .AddUserManager<UserManager<ApplicationUser>>()
            .AddRoleManager<RoleManager<ApplicationRole>>();
        
        //注入Identity服务
        services.AddTransient<IIdentityService, IdentityService>();
        
        return services;
    }
}