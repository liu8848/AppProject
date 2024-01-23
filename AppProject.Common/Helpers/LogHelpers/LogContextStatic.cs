namespace AppProject.Common.Helpers.LogHelpers;

public class LogContextStatic
{
    public static readonly string FileMessageTemplate = "{NewLine}Date：{Timestamp:yyyy-MM-dd HH:mm:ss.fff}{NewLine}IP:{request_path}{NewLine}ThreadId:{ThreadId}{NewLine}LogLevel：{Level}{NewLine}SourceContext:{SourceContext} {NewLine}Message：{Message:l}{NewLine}{Exception}{NewLine}{Test}" + new string('-', 100);

    public static readonly string SqlMessageTemplate = "{NewLine}Date: {Timestamp:yyyy-MM-dd HH:mm:ss.fff}{NewLine}LogLevel:{Level}{NewLine}Table:{Table}{NewLine}Parameters:{Parameters} {NewLine}SQL:{SQL:l} {NewLine}{Exception}"+new string('-',100);

}