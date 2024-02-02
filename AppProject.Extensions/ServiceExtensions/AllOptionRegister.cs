using AppProject.Common;
using AppProject.Common.Option.Core;
using Microsoft.Extensions.DependencyInjection;

namespace AppProject.Extensions.ServiceExtensions;

public static class AllOptionRegister
{
    public static void AddAllOptionRegister(this IServiceCollection services)
    {
        if (services is null) throw new ArgumentException(nameof(services));

        var optionTypes = App.EffectiveTypes
            .Where(s=>!s.IsInterface&&typeof(IConfigurableOptions).IsAssignableFrom(s));

        foreach (var optionType in optionTypes )
        {
            services.AddConfigurableOptions(optionType);
        }
    }
}