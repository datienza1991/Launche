using Microsoft.Extensions.DependencyInjection;
using UI.Basic.Project.Command;
using UI.Basic.Project.Queries;

namespace UI.Basic.Project;

public static class ServiceCollection
{
    public static IServiceCollection AddProjectServiceCollection(this IServiceCollection services)
    {
        return services
            .AddSingleton<IProjectCommand, ProjectCommand>()
            .AddSingleton<IProjectQuery, ProjectQuery>();
    }
}

