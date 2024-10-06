using ApplicationCore.Features;
using Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace ApplicationCore;

public static class ApplicationCoreServiceCollection
{
    public static IServiceCollection AddApplicationCoreServiceCollection(this IServiceCollection services)
    {
        return services
            .AddApplicationFeatureServiceCollection()
            .AddInfrastructureServiceCollection()
            .AddSingleton<IStartup, Startup>();
    }
}

