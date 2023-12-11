using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using UI.Database;
using UI.VsCodePath;

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
            var services = new ServiceCollection();

            var serviceProvider = services
                .AddSingleton<IAddTableSchemaVersion, AddTableSchemaVersion>()
                .AddSingleton<ICheckVersionIfExists, CheckVersionIfExists>()
                .AddSingleton<ICheckVersionTableIfExists, CheckVersionTableIfExists>()
                .AddSingleton<ICreateSqliteConnection, CreateSqliteConnection>()
                .AddSingleton<ICreateVersionsDbTable, CreateVersionsDbTable>()
                .AddSingleton<IInitializedDatabaseMigration, InitializedDatabaseMigration>()
                .AddSingleton<ISaveVsCodePath, SaveVsCodePath>()
                .AddSingleton<IGetVsCodePath, GetVsCodePath>()
                .AddSingleton<MainWindow>()
                .BuildServiceProvider();

            var mainWindow = serviceProvider.GetService<MainWindow>();
            mainWindow?.Show();
        }
    }

}
