using ApplicationCore.Common;
using System.Diagnostics;

namespace ApplicationCore.Features.Projects;

public class OpenProjectDevAppCommand
{
    public bool HasFileName { get; set; }
    public string FullFilePath { get; set; } = string.Empty;
    public string DirectoryPath { get; set; } = string.Empty;
    public string DevAppPath { get; set; } = string.Empty;
}

public interface IOpenProjectDevAppService
{
    void Handle(OpenProjectDevAppCommand command);
}

internal class OpenProjectDevAppService(INotificationMessageService notificationMessageService) : IOpenProjectDevAppService
{
    private readonly INotificationMessageService notificationMessageService = notificationMessageService;

    public void Handle(OpenProjectDevAppCommand command)
    {
        try
        {
            if (!Directory.Exists(command.DirectoryPath))
            {
                this.notificationMessageService.Create("Directory not found!", "Launch IDE Error", NotificationType.Error);
                return;
            }

            if (command.HasFileName)
            {
                this.OpenIDEWithFileName(command.FullFilePath, command.DevAppPath);
                return;
            }

            OpenIDE
            (
                new()
                {
                    FileName = command.DevAppPath,
                    Arguments = command.DirectoryPath,
                    UseShellExecute = true,
                }
            );
        }
        catch (Exception ex)
        {
            notificationMessageService.Create(ex.Message, "Open Project Dev App", NotificationType.Error);
        }
    }

    private void OpenIDEWithFileName(string fullFilePath, string devAppPath)
    {
        if (File.Exists(fullFilePath) is false)
        {
            notificationMessageService.Create("File not found!", "Open Project Error", NotificationType.Error);
            return;
        }

        OpenIDE
        (
            new()
            {
                FileName = devAppPath,
                Arguments = $"{fullFilePath}",
                UseShellExecute = true,
            }
        );
    }

    private static void OpenIDE(ProcessStartInfo processInfo)
    {
        using Process process = new();
        process.StartInfo = processInfo;
        process.Start();
    }
}

