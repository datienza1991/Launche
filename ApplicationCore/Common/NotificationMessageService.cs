namespace ApplicationCore.Common;

public enum NotificationType
{
    None = 0,
    Success = 1,
    Warning = 2,
    Error = 3,
    Information = 4,
}

public class NotificationMessageEventArgs(
    string message,
    string title,
    NotificationType notificationType
) : EventArgs
{
    public string Message { get; } = message;
    public string Title { get; } = title;
    public NotificationType NotificationType { get; } = notificationType;
}

public interface INotificationMessageService
{
    event EventHandler<NotificationMessageEventArgs>? Notify;
    void Create(string message, string title, NotificationType notificationType);
}

public class NotificationMessageService : INotificationMessageService
{
    public event EventHandler<NotificationMessageEventArgs>? Notify;

    public void Create(string message, string title, NotificationType notificationType)
    {
        Notify!.Invoke(this, new(message, title, notificationType));
    }
}
