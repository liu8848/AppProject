using AppProject.Common.Option.Core;

namespace AppProject.Common.Option;

public class JwtSettingsOptions:IConfigurableOptions
{
    public string ValidIssuer { get; set; }
    public string ValidAudience { get; set; }
    public long Expires { get; set; }
    public string SecretKey { get; set; }
    
    
}