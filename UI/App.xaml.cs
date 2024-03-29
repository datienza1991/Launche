﻿using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using UI.Database;
using UI.ProjectPath;
using UI.IDEPath;

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
                .BuildServiceProvider();

            var mainWindow = serviceProvider.GetService<MainWindow>();
            mainWindow?.Show();
        }
    }

}
