using System.Data.Common;
using Serilog.Core;
using Serilog.Events;

namespace AppProject.Repository;

public class DbCommandEnricher:ILogEventEnricher
{
    private readonly DbCommand _command;
    private readonly Action<LogEvent, ILogEventPropertyFactory, DbCommand> _action;

    public DbCommandEnricher(DbCommand command):this(command,null)
    {
    }

    public DbCommandEnricher(DbCommand Command,
    Action<LogEvent, ILogEventPropertyFactory, DbCommand> action)
    {
        _command = Command;
        if (action == null)
        {
            _action = (logEvent, logEventPropertyFactory, command) =>
            {
                logEvent.AddPropertyIfAbsent(logEventPropertyFactory.CreateProperty("logType","SQL"));
                logEvent.AddPropertyIfAbsent(logEventPropertyFactory.CreateProperty("Command_Text",command.CommandText));
            };
        }
        else
        {
            _action = action;
        }
        
    }

    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        // if (_command is not null)
        // {
        //     _action.Invoke(logEvent,propertyFactory,_command);
        // }
        
        _action.Invoke(logEvent,propertyFactory,_command);
    }
}