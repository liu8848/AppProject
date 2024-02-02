using System.Reflection;
using AppProject.Common.Core;
using AppProject.Common.Extensions;
using AppProject.Common.Option.Core;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

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
    public static IServiceProvider RootService => IsRun || IsBuild ? InternalApp.RootServices : null;


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

    /// <summary>获取请求生存周期的服务</summary>
    /// <param name="type"></param>
    /// <param name="serviceProvider"></param>
    /// <param name="mustBuild"></param>
    /// <returns></returns>
    public static TService GetService<TService>(bool mustBuild = true)
        where TService : class => App.GetService(typeof(TService), null, mustBuild) as TService;
    public static TService? GetService<TService>(IServiceProvider serviceProvider, bool mustBuild = true)
        where TService : class => (serviceProvider ?? App.GetServiceProvider(typeof(TService), mustBuild, false))
        ?.GetService<TService>();
    public static object? GetService(Type type, IServiceProvider serviceProvider = null, bool mustBuild = true) =>
        (serviceProvider ?? App.GetServiceProvider(type, mustBuild, false))?.GetService(type);

    #endregion


    #region Options获取配置选项类

    /// <summary>
    /// 获取配置
    /// </summary>
    /// <typeparam name="TOptions">强类型选项类</typeparam>
    /// <returns>TOptions</returns>
    public static TOptions GetConfig<TOptions>()
    where TOptions:class,IConfigurableOptions
    {
        var instance = Configuration
            .GetSection(ConfigurableOptions.GetConfigurationPath(typeof(TOptions)))
            .Get<TOptions>();
        return instance;
    }

    /// <summary>
    /// 获取选项 IOptions生命周期为Singleton,无法热更新
    /// </summary>
    /// <param name="serviceProvider"></param>
    /// <typeparam name="TOptions"></typeparam>
    /// <returns></returns>
    public static TOptions? GetOptions<TOptions>(IServiceProvider serviceProvider = null)
        where TOptions : class, new()
    {
        IOptions<TOptions>? service = GetService<IOptions<TOptions>>(serviceProvider ??
                                                                     App.RootService, false);
        return service?.Value;
    }

    /// <summary>
    /// 获取选项 IOptionsMonitor生命周期为Singleton，支持热更新
    /// </summary>
    /// <param name="serviceProvider"></param>
    /// <typeparam name="TOptions"></typeparam>
    /// <returns></returns>
    public static TOptions? GetOptionsMonitor<TOptions>(IServiceProvider serviceProvider = null)
    where TOptions:class,new()
    {
        var service = App.GetService<IOptionsMonitor<TOptions>>(serviceProvider ??
                                                                       App.RootService, false);
        return service?.CurrentValue;
    }

    /// <summary>
    /// 获取选项 IOptionsSnapshot生命周期为scope，每次请求创建新的选项类，并读取最新配置
    /// </summary>
    /// <param name="serviceProvider"></param>
    /// <typeparam name="TOptions"></typeparam>
    /// <returns></returns>
    public static TOptions? GetOptionsSnapshot<TOptions>(IServiceProvider serviceProvider = null)
    where TOptions:class,new()
    {
        var service = GetService<IOptionsSnapshot<TOptions>>(serviceProvider??
                                                                     App.RootService,false);
        return service?.Value;
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