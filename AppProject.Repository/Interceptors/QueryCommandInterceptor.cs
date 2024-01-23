using System.Data.Common;
using AppProject.Common.LogHelper;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Serilog;
using Serilog.Context;
using Serilog.Core;

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

    public void LogDbCommand(CommandEventData eventData)
    {
        using (LogContextExtension.Create.SqlAopPushProperty(eventData))
        {
            // Log.Information($"Type:{command.CommandType}\r\nSQL:{command.CommandText}\r\n");
            Log.Information("执行SQL");
        }
    }
}