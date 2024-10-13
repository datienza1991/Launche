using ApplicationCore.Common;
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
            .AddSingleton<IGroupQuery, GroupQuery>()
            .AddSingleton<IDevAppFeaturesCreator, DevAppFeaturesCreator>()
            .AddSingleton<INotificationMessageService, NotificationMessageService>()
            .AddSingleton<IProjectFeaturesCreator, ProjectFeaturesCreator>();
    }
}

