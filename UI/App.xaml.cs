using ApplicationCore;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using UI.Windows.Group;

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
            var services = new ServiceCollection();

            return services
                 .AddApplicationCoreServiceCollection()
                 .AddSingleton<MainWindow>()
                 .AddSingleton<GroupModalWindow>()
                 .BuildServiceProvider();
        }
    }

}
