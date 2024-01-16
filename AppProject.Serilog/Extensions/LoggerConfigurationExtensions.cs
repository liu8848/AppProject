using AppProject.Common.Helpers.LogHelpers;
using Serilog;

namespace AppProject.Serilog.Extensions;

public static class LoggerConfigurationExtensions
{
    public static LoggerConfiguration WriteToConsole(this LoggerConfiguration loggerConfiguration)
    {
        loggerConfiguration = loggerConfiguration.WriteTo.Logger(lg =>
            lg.WriteTo.Console(outputTemplate:LogContextStatic.FileMessageTemplate));
        return loggerConfiguration;
    }
}