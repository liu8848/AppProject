using AppProject.Common;
using AppProject.Common.Extensions.Redis;
using AppProject.Common.Option;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;

namespace AppProject.Extensions.ServiceExtensions;

public static class CacheSetup
{
    /// <summary>
    /// 统一注册缓存
    /// </summary>
    /// <param name="services"></param>
    public static void AddCacheSetup(this IServiceCollection services)
    {
        var redisOptions = App.GetOptions<RedisOptions>()??throw new ArgumentException(nameof(RedisOptions));
        // var redisOptions = App.GetOptionsMonitor<RedisOptions>()??throw new ArgumentException(nameof(RedisOptions));
        //是否启用redis
        if (redisOptions.Enable)
        {
            services.AddSingleton<IConnectionMultiplexer>(sp =>
            {
                var redisConfiguration = ConfigurationOptions.Parse(redisOptions.ConnectionString, true);
                redisConfiguration.ResolveDns = true;
                return ConnectionMultiplexer.Connect(redisConfiguration);
            });
            services.AddSingleton<ConnectionMultiplexer>(p =>
                p.GetService<IConnectionMultiplexer>() as ConnectionMultiplexer);
            //使用redis
            services.AddStackExchangeRedisCache(options =>
            {
                options.ConnectionMultiplexerFactory = () => Task.FromResult(App.GetService<IConnectionMultiplexer>());
                if (!redisOptions.InstanceName.IsNullOrEmpty())
                    options.InstanceName = redisOptions.InstanceName;
            });
            services.AddTransient<IRedisBasketRepository, RedisBasketRepository>();
        }
    }
}