namespace ApplicationCore.Common;

public enum NotificationType
{
    None = 0,
    Success = 1,
    Warning = 2,
    Error = 3,
    Information = 4,
}
public class NotificationMessageEventArgs
(
    string message,
    string title,
    NotificationType notificationType
)
    : EventArgs
{
    public string Message { get; } = message;
    public string Title { get; } = title;
    public NotificationType NotificationType { get; } = notificationType;
}

public interface INotificationMessageService
{
    event EventHandler<NotificationMessageEventArgs>? Notify;
    void Create(string message, string title, NotificationType notificationType);
    EventHandler<NotificationMessageEventArgs> GetEvent();
}

internal class NotificationMessageService : INotificationMessageService
{
    private EventHandler<NotificationMessageEventArgs>? _onNotifyOccured;

    public event EventHandler<NotificationMessageEventArgs>? Notify
    {
        add { _onNotifyOccured += value; }
        remove { _onNotifyOccured -= value; }
    }

    public void Create(string message, string title, NotificationType notificationType)
    {
        _onNotifyOccured!.Invoke(this, new(message, title, notificationType));
    }

    public EventHandler<NotificationMessageEventArgs> GetEvent()
    {
        return _onNotifyOccured ?? delegate { };
    }
}

