using AppProject.Serilog.Extensions;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace AppProject.Extensions.ServiceExtensions;

/// <summary>
/// Serilog配置类
/// </summary>
public static class SerilogSetup
{
    public static IHostBuilder AddSerilogSetup(this IHostBuilder host)
    {
        if (host == null) throw new ArgumentNullException(nameof(host));

        LoggerConfiguration loggerConfiguration = new LoggerConfiguration()
            // .ReadFrom.Configuration(AppSettings.Configuration)
            .Enrich.FromLogContext()
            .WriteToConsole();
        
        Log.Logger=loggerConfiguration.CreateLogger();

        host.UseSerilog();
        return host;
    }
}