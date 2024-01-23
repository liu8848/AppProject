using AppProject.Common.Helpers.LogHelpers;
using Serilog;
using Serilog.Events;
using Serilog.Filters;

namespace AppProject.Serilog.Extensions;

public static class LoggerConfigurationExtensions
{
    public static LoggerConfiguration WriteToConsole(this LoggerConfiguration loggerConfiguration)
    {
        loggerConfiguration = loggerConfiguration.WriteTo.Logger(lg =>
            lg
                .Filter.ByIncludingOnly(Matching.WithProperty<string>("LogType",s=>s.Equals("DbCommand")))
                .WriteTo.Console(
                outputTemplate:LogContextStatic.SqlMessageTemplate));
        return loggerConfiguration;
    }
}