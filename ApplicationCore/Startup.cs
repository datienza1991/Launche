using Infrastructure.Database;

namespace ApplicationCore;

public interface IStartup
{
    Task Init();
}
internal class Startup(IInitializedDatabaseMigration initializedDatabaseMigration) : IStartup
{
    private readonly IInitializedDatabaseMigration initializedDatabaseMigration = initializedDatabaseMigration;

    public async Task Init()
    {
        await initializedDatabaseMigration.Execute();
    }
}

