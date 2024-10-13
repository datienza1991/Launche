using ApplicationCore.Common;
using System.Diagnostics;

namespace ApplicationCore.Features.Projects;
public class OpenProjectFolderWindowCommand
{
    public string Path { get; set; } = "";
}

public interface IOpenProjectFolderWindowService
{
    void Handle(OpenProjectFolderWindowCommand command);
}

internal class OpenProjectFolderWindowService(INotificationMessageService notificationMessageService) : IOpenProjectFolderWindowService
{
    private readonly INotificationMessageService notificationMessageService = notificationMessageService;

    public void Handle(OpenProjectFolderWindowCommand command)
    {
        try
        {
            if (!Directory.Exists(command.Path))
            {
                this.notificationMessageService.Create("Directory not found!", "Open Project", NotificationType.Error);
                return;
            }
            ProcessStartInfo startInfo = new()
            {
                FileName = "explorer.exe",
                Arguments = command.Path,
                UseShellExecute = true,
            };
            Process.Start(startInfo);
        }
        catch (Exception ex)
        {
            notificationMessageService.Create(ex.Message, "Open Project", NotificationType.Error);
        }
    }
}

