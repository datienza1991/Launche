using Microsoft.Extensions.DependencyInjection;
using UI.Queries.Group;

namespace UI.Queries.Project;

public static class ServiceCollection
{
    public static IServiceCollection AddQueriesServiceCollection(this IServiceCollection services)
    {
        return services
            .AddSingleton<IProjectQuery, ProjectQuery>()
            .AddSingleton<IGroupQuery, GroupQuery>();
    }
}

