using ApplicationCore.Features.DevApps;
using ApplicationCore.Features.Grouping;
using ApplicationCore.Features.Groups;
using ApplicationCore.Features.Projects;
using ApplicationCore.Features.Sorting;
using Microsoft.Extensions.DependencyInjection;

namespace ApplicationCore.Features;

public static class CoreServiceCollection
{
    public static IServiceCollection AddApplicationFeatureServiceCollection(this IServiceCollection services)
    {
        return services
            .AddSingleton<ISortProject, SortProject>()
            .AddSingleton<IGroupProject, GroupProject>()
            .AddSingleton<IDevAppCommand, DevAppCommand>()
            .AddSingleton<IDevAppQuery, DevAppQuery>()
            .AddSingleton<IGroupQuery, GroupQuery>()
            .AddSingleton<IProjectCommand, ProjectCommand>()
            .AddSingleton<IProjectQuery, ProjectQuery>();
    }
}

