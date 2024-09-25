using Microsoft.Extensions.DependencyInjection;
using UI.Basic.Project.Data;

namespace UI.Basic.Project;

public static class ServiceCollection
{
    public static IServiceCollection AddProjectServiceCollection(this IServiceCollection services)
    {
        return services
            .AddSingleton<IPersistence, Persistence>()
            .AddSingleton<IQuery, Query>();
    }
}

