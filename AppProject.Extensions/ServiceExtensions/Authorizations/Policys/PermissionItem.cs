namespace AppProject.Extensions.ServiceExtensions.Authorizations.Policys;

public class PermissionItem
{
    /// <summary>
    /// 用户或角色获取其他凭据名称
    /// </summary>
    public virtual string Role { get; set; }
    /// <summary>
    /// 用户名
    /// </summary>
    public virtual string UserName { get; set; }
    /// <summary>
    /// 请求Url
    /// </summary>
    public virtual string Url { get; set; }
}