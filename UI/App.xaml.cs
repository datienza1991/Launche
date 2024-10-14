using ApplicationCore;
using ApplicationCore.Common;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace UI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected async override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            var serviceProvider = GetCurrentServiceProvider();
            var mainWindow = serviceProvider.GetService<MainWindow>();
            var notificationService = serviceProvider.GetService<INotificationMessageService>();
            var startup = serviceProvider.GetService<IStartup>();
            notificationService!.Notify += NotificationService_Notify;
            await startup!.Init();
            mainWindow?.Show();
        }

        private void NotificationService_Notify(object? sender, NotificationMessageEventArgs e)
        {
            var messageBoxIcon = MessageBoxImage.None;

            switch (e.NotificationType)
            {
                case NotificationType.None:
                    messageBoxIcon = MessageBoxImage.None;
                    break;
                case NotificationType.Success:
                    messageBoxIcon = MessageBoxImage.Information;
                    break;
                case NotificationType.Warning:
                    messageBoxIcon = MessageBoxImage.Warning;
                    break;
                case NotificationType.Error:
                    messageBoxIcon = MessageBoxImage.Error;
                    break;
                case NotificationType.Information:
                    messageBoxIcon = MessageBoxImage.Information;
                    break;
            }

            MessageBox.Show(e.Message, e.Title, MessageBoxButton.OK, messageBoxIcon);
        }

        public static ServiceProvider GetCurrentServiceProvider()
        {
            var services = new ServiceCollection();

            return services
                 .AddApplicationCoreServiceCollection()
                 .AddSingleton<MainWindow>()
                 .BuildServiceProvider();
        }
    }

}
