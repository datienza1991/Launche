using ApplicationCore.Features.Basic.Group;
using ApplicationCore.Features.Basic.Project;
using ApplicationCore.Features.Grouping;
using ApplicationCore.Features.Sorting;
using Microsoft.Extensions.DependencyInjection;

namespace ApplicationCore.Features;

public static class ServiceCollection
{
    public static IServiceCollection AddFeaturesServiceCollection(this IServiceCollection services)
    {
        return services
            .AddSingleton<IGroupDataService, GroupDataService>()
            .AddSingleton<IProjectDataService, ProjectDataService>()
            .AddSingleton<ISortProject, SortProject>()
            .AddSingleton<IGroupProject, GroupProject>();
    }
}

