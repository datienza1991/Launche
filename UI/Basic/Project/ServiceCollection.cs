using Microsoft.Extensions.DependencyInjection;

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

