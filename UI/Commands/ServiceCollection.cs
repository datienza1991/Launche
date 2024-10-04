using Microsoft.Extensions.DependencyInjection;

namespace UI.Commands;

public static class ServiceCollection
{
    public static IServiceCollection AddCommandsServiceCollection(this IServiceCollection services)
    {
        return services
            .AddSingleton<Basic.Project.IProjectCommand, Basic.Project.ProjectCommand>()
            .AddSingleton<Sorting.IProjectCommand, Sorting.ProjectCommand>()
            .AddSingleton<Grouping.IProjectCommand, Grouping.ProjectCommand>();
    }
}

