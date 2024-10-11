using ApplicationCore.Features.Groups;
using ApplicationCore.Features.Projects;
using Infrastructure.Database;
using Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class InfrastructureServiceCollection
{
    public static IServiceCollection AddInfrastructureServiceCollection(this IServiceCollection services)
    {
        return services
            .AddSingleton<IAddTableSchemaVersion, AddTableSchemaVersion>()
            .AddSingleton<ICheckVersionIfExists, CheckVersionIfExists>()
            .AddSingleton<ICheckVersionTableIfExists, CheckVersionTableIfExists>()
            .AddSingleton<ICreateSqliteConnection, CreateSqliteConnection>()
            .AddSingleton<ICreateVersionsDbTable, CreateVersionsDbTable>()
            .AddSingleton<IInitializedDatabaseMigration, InitializedDatabaseMigration>()
            .AddSingleton<IGitService, GitService>()
            .AddSingleton<IGroupRepository, GroupRepository>()
            .AddSingleton<IDevAppRepository, DevAppRepository>()
            .AddSingleton<IProjectRepository, ProjectRepository>();
    }
}

