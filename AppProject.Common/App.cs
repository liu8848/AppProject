using System.Reflection;
using AppProject.Common.Core;
using AppProject.Common.Extensions;
using AppProject.Common.Option.Core;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AppProject.Common;

public class App
{
    /// <summary>
    ///     是否正在运行
    /// </summary>
    private static bool _isRun;

    /// <summary>
    ///     应用有效程序集
    /// </summary>
    public static readonly IEnumerable<Assembly> Assemblies = RuntimeExtension.GetAllAssemblies();

    /// <summary>
    ///     有效程序集类型
    /// </summary>
    public static readonly IEnumerable<Type> EffectiveTypes;

    static App()
    {
        EffectiveTypes = Assemblies.SelectMany(GetTypes);
    }

    public static bool IsBuild { get; set; }

    public static bool IsRun
    {
        get => _isRun;
        set => _isRun = IsBuild = value;
    }

    /// <summary>
    ///     优先使用App.GetService()手动获取服务
    /// </summary>
    public static IServiceProvider? RootService => IsRun || IsBuild ? InternalApp.RootServices : null;


    /// <summary>
    ///     获取Web主机环境
    /// </summary>
    public static IWebHostEnvironment WebHostEnvironment => InternalApp.WebHostEnvironment;


    /// <summary>
    ///     获取泛型主机环境
    /// </summary>
    public static IHostEnvironment HostEnvironment => InternalApp.HostEnvironment;


    /// <summary>
    ///     全局配置选项
    /// </summary>
    public static IConfiguration Configuration => InternalApp.Configuration;

    #region Service获取方法

    public static IServiceProvider GetServiceProvider(Type serviceType, bool mustBuild = false,
        bool throwException = true)
    {
        if (HostEnvironment == null || (RootService != null &&
                                        InternalApp.InternalServices
                                            .Where(u => u.ServiceType ==
                                                        (serviceType.IsGenericType
                                                            ? serviceType.GetGenericTypeDefinition()
                                                            : serviceType))
                                            .Any(u => u.Lifetime == ServiceLifetime.Singleton)))
            return RootService;

        var serviceProvider = InternalApp.InternalServices.BuildServiceProvider();
        return serviceProvider;
    }

    #endregion


    #region Options获取配置选项类

    public static TOptions? GetConfig<TOptions>()
    {
        var instance = Configuration
            .GetSection(ConfigurableOptions.GetConfigurationPath(typeof(TOptions)))
            .Get<TOptions>();
        return instance;
    }

    #endregion

    #region private方法

    /// <summary>
    ///     加载程序集中所有类型
    /// </summary>
    /// <param name="assembly"></param>
    /// <returns></returns>
    private static IEnumerable<Type> GetTypes(Assembly assembly)
    {
        var source = Array.Empty<Type>();
        try
        {
            source = assembly.GetTypes();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return source.Where(u => u.IsPublic);
    }

    #endregion
}