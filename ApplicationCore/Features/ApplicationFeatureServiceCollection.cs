using ApplicationCore.Common;
using ApplicationCore.Features.DevApps;
using ApplicationCore.Features.Git;
using ApplicationCore.Features.Groups;

using ApplicationCore.Features.Projects;
using Microsoft.Extensions.DependencyInjection;

namespace ApplicationCore.Features;

public static class CoreServiceCollection
{
    public static IServiceCollection AddApplicationFeatureServiceCollection(this IServiceCollection services)
        => services
            .AddSingleton<IGroupFeaturesCreator, GroupFeaturesCreator>()
            .AddSingleton<IDevAppFeaturesCreator, DevAppFeaturesCreator>()
            .AddSingleton<INotificationMessageService, NotificationMessageService>()
            .AddSingleton<IGitFeaturesCreator, GitFeaturesCreator>()
            .AddSingleton<IProjectFeaturesCreator, ProjectFeaturesCreator>();
}

