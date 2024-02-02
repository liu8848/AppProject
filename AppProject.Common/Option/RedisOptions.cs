using AppProject.Common.Option.Core;

namespace AppProject.Common.Option;

/// <summary>
/// 缓存配置选项
/// </summary>
public class RedisOptions:IConfigurableOptions
{
    /// <summary>
    /// 是否启用
    /// </summary>
    public bool Enable { get; set; }

    /// <summary>
    /// redis连接字符串
    /// </summary>
    public string ConnectionString { get; set; }
    
    
    /// <summary>
    /// 键值前缀
    /// </summary>
    public string InstanceName { get; set; }
}