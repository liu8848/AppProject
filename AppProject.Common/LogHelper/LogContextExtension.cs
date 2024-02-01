using System.Data.Common;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Serilog.Context;

namespace AppProject.Common.LogHelper;

public class LogContextExtension:IDisposable
{
    private readonly Stack<IDisposable> _disposableStack = new Stack<IDisposable>();

    public static LogContextExtension Create => new();

    public void AddStock(IDisposable disposable)
    {
        _disposableStack.Push(disposable);
    }

    public IDisposable SqlAopPushProperty(CommandEventData eventData)
    {
        DbParameterCollection dbParameterCollection = eventData.Command.Parameters;
        AddStock(LogContext.PushProperty("LogType","DbCommand"));
        AddStock(LogContext.PushProperty("Table",ExtractTableName(eventData.Command.CommandText)));
        AddStock(LogContext.PushProperty("Parameters",GetParasStr(eventData.Command.Parameters)));
        AddStock(LogContext.PushProperty("SQL",eventData.Command.CommandText));

        return this;
    }

    public void Dispose()
    {
        while (_disposableStack.Count > 0)
        {
            _disposableStack.Pop().Dispose();
        }
    }

    private static string GetParasStr(DbParameterCollection paras)
    {
        string str = string.Empty;
        foreach (DbParameter para in paras)
        {
            str += $"{para.ParameterName}:{para.Value}\r\n";
        }

        return str;
    }
    
    private static string ExtractTableName(string sql)
    {
        // 匹配 SQL 语句中的表名的正则表达式
        //string regexPattern = @"\s*(?:UPDATE|DELETE\s+FROM|SELECT\s+\*\s+FROM)\s+(\w+)";
        string regexPattern = @"(?i)(?:FROM|UPDATE|DELETE\s+FROM)\s+`(.+?)`";
        Regex regex = new Regex(regexPattern, RegexOptions.IgnoreCase);
        Match match = regex.Match(sql);

        if (match.Success)
        {
            // 提取匹配到的表名
            return match.Groups[1].Value;
        }
        else
        {
            // 如果没有匹配到表名，则返回空字符串或者抛出异常等处理
            return string.Empty;
        }
    }
}