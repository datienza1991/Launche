using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using UI.Commands;
using UI.Database;
using UI.IDEPath;
using UI.Queries.Project;

namespace UI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            var mainWindow = GetCurrentServiceProvider().GetService<MainWindow>();
            mainWindow?.Show();
        }

        public static ServiceProvider GetCurrentServiceProvider()
        {
            var services = new Microsoft.Extensions.DependencyInjection.ServiceCollection();

            return services
                 .AddQueriesServiceCollection()
                 .AddCommandsServiceCollection()
                 .AddSingleton<IAddTableSchemaVersion, AddTableSchemaVersion>()
                 .AddSingleton<ICheckVersionIfExists, CheckVersionIfExists>()
                 .AddSingleton<ICheckVersionTableIfExists, CheckVersionTableIfExists>()
                 .AddSingleton<ICreateSqliteConnection, CreateSqliteConnection>()
                 .AddSingleton<ICreateVersionsDbTable, CreateVersionsDbTable>()
                 .AddSingleton<IInitializedDatabaseMigration, InitializedDatabaseMigration>()
                 .AddSingleton<ISaveIDEPath, SaveIDEPath>()
                 .AddSingleton<IGetIDEPath, GetIDEPath>()
                 .AddSingleton<IGetIDEPaths, GetIDEPaths>()
                 .AddSingleton<IDeleteIdePath, DeleteIdePath>()
                 .AddSingleton<MainWindow>()
                 .AddSingleton<GroupModalWindow>()
                 .BuildServiceProvider();
        }
    }

}
