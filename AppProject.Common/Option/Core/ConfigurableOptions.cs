using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace AppProject.Common.Option.Core;

public static class ConfigurableOptions
{
    /// <summary>
    /// 添加选项配置
    /// </summary>
    /// <param name="services">服务集合</param>
    /// <typeparam name="TOptions">选项类型</typeparam>
    /// <returns></returns>
    public static IServiceCollection AddConfigurableOptions<TOptions>(this IServiceCollection services)
    where TOptions:class,IConfigurableOptions
    {
        var optionsType = typeof(TOptions);
        string path = GetConfigurationPath(optionsType);
        services.Configure<TOptions>(App.Configuration.GetSection(path));
        return services;
    }

    public static IServiceCollection AddConfigurableOptions(this IServiceCollection services, Type type)
    {
        string path = GetConfigurationPath(type);
        var config = App.Configuration.GetSection(path);

        var iOptionsChangeTokenSource = typeof(IOptionsChangeTokenSource<>);
        var iConfigureOptions = typeof(IConfigureOptions<>);
        var configurationChangeTokenSource = typeof(ConfigurationChangeTokenSource<>);
        var namedConfigureFromConfigurationOptions = typeof(NamedConfigureFromConfigurationOptions<>);
        iOptionsChangeTokenSource = iOptionsChangeTokenSource.MakeGenericType(type);
        iConfigureOptions = iConfigureOptions.MakeGenericType(type);
        configurationChangeTokenSource = configurationChangeTokenSource.MakeGenericType(type);
        namedConfigureFromConfigurationOptions = namedConfigureFromConfigurationOptions.MakeGenericType(type);

        services.AddOptions();
        services.AddSingleton(iOptionsChangeTokenSource,
            Activator.CreateInstance(configurationChangeTokenSource, Options.DefaultName, config) ??
            throw new InvalidOperationException());
        return services.AddSingleton(iConfigureOptions,
            Activator.CreateInstance(namedConfigureFromConfigurationOptions, Options.DefaultName, config)
            ?? throw new InvalidOperationException());
    }
    
    
    /// <summary>
    /// 获取配置类名称前缀，作为json搜索的section名
    /// </summary>
    /// <param name="optionsType"></param>
    /// <returns></returns>
    public static string GetConfigurationPath(Type optionsType)
    {
        var endPath = new[] { "Options", "Option" };
        var configurationPath = optionsType.Name;
        foreach (var s in endPath)
            if (configurationPath.EndsWith(s))
                return configurationPath[..^s.Length];

        return configurationPath;
    }
}