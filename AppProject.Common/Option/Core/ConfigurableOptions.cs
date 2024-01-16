namespace AppProject.Common.Option.Core;

public static class ConfigurableOptions
{
    public static string GetConfigurationPath(Type optionsType)
    {
        var endPath = new[] { "Options", "Option" };
        var configurationPath = optionsType.Name;
        foreach (var s in endPath)
            if (configurationPath.EndsWith(s))
                return configurationPath[..^s.Length];

        return configurationPath;
    }
}