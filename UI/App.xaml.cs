using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using UI.Basic.Project;
using UI.Database;
using UI.Group;
using UI.IDEPath;
using UI.ProjectPath;

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
                 .AddGroupServiceCollection()
                 .AddProjectServiceCollection()
                 .AddSingleton<IAddTableSchemaVersion, AddTableSchemaVersion>()
                 .AddSingleton<ICheckVersionIfExists, CheckVersionIfExists>()
                 .AddSingleton<ICheckVersionTableIfExists, CheckVersionTableIfExists>()
                 .AddSingleton<ICreateSqliteConnection, CreateSqliteConnection>()
                 .AddSingleton<ICreateVersionsDbTable, CreateVersionsDbTable>()
                 .AddSingleton<IInitializedDatabaseMigration, InitializedDatabaseMigration>()
                 .AddSingleton<ISaveIDEPath, SaveIDEPath>()
                 .AddSingleton<IGetIDEPath, GetIDEPath>()
                 .AddSingleton<IGetProjectPaths, GetProjectPaths>()
                 .AddSingleton<IAddProjectPath, AddProjectPath>()
                 .AddSingleton<IEditProjectPath, EditProjectPath>()
                 .AddSingleton<IGetIDEPaths, GetIDEPaths>()
                 .AddSingleton<IGetLastProjectPath, GetLastProjectPath>()
                 .AddSingleton<IDeleteProjectPath, DeleteProjectPath>()
                 .AddSingleton<IDeleteIdePath, DeleteIdePath>()
                 .AddSingleton<ISortUpProjectPath, SortUpProjectPath>()
                 .AddSingleton<ISortDownProjectPath, SortDownProjectPath>()
                 .AddSingleton<MainWindow>()
                 .AddSingleton<GroupModalWindow>()
                 .BuildServiceProvider();
        }
    }

}
