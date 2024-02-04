using System.Data.Common;
using AppProject.Common.LogHelper;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Serilog;

namespace AppProject.Repository.Interceptors;

public class QueryCommandInterceptor:DbCommandInterceptor
{

    public override DbDataReader ReaderExecuted(DbCommand command, 
        CommandExecutedEventData eventData, 
        DbDataReader result)
    {
        LogDbCommand(eventData);
        return result;
    }

    public override ValueTask<DbDataReader> ReaderExecutedAsync(DbCommand command, 
        CommandExecutedEventData eventData, 
        DbDataReader result,
        CancellationToken cancellationToken = new CancellationToken())
    {
        LogDbCommand(eventData);
        return new ValueTask<DbDataReader>(result);
    }

    private static void LogDbCommand(CommandEventData eventData)
    {
        using (LogContextExtension.Create.SqlAopPushProperty(eventData))
        {
            Log.Information("执行SQL");
        }
    }
}