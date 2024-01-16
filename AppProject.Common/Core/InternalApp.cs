using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AppProject.Common.Core;

/// <summary>
///     APP内部信息
/// </summary>
public static class InternalApp
{
    internal static IServiceCollection InternalServices;

    /// <summary>
    ///     根服务
    /// </summary>
    internal static IServiceProvider RootServices;

    /// <summary>
    ///     获取Web主机环境
    /// </summary>
    internal static IWebHostEnvironment WebHostEnvironment;


    /// <summary>
    ///     获取泛型主机环境
    /// </summary>
    internal static IHostEnvironment HostEnvironment;

    /// <summary>
    ///     配置对象
    /// </summary>
    internal static IConfiguration Configuration;


    /// <summary>
    ///     扩展方法，将宿主环境参数加载至全局变量中，方便读取
    /// </summary>
    /// <param name="wab"></param>
    public static void ConfigureApplication(this WebApplicationBuilder wab)
    {
        HostEnvironment = wab.Environment;
        WebHostEnvironment = wab.Environment;
        InternalServices = wab.Services;
    }

    public static void ConfigureApplication(this IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public static void ConfigureApplication(this IHost app)
    {
        RootServices = app.Services;
    }
}