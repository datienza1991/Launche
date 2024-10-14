using ApplicationCore.Common;

namespace ApplicationCore.Features.Projects;

public class RemoveProjectFromGroupCommand
{
    public long ProjectId { get; set; }
}

public interface IRemoveProjectFromGroupService
{
    Task Handle(RemoveProjectFromGroupCommand command);
}
internal class RemoveProjectFromGroupService
(
    IProjectRepository projectRepository,
    INotificationMessageService notificationMessageService,
    IRemoveProjectFromGroupNotificationService removeProjectFromGroupNotificationService
)
    : IRemoveProjectFromGroupService
{
    private readonly IProjectRepository projectRepository = projectRepository;
    private readonly INotificationMessageService notificationMessageService = notificationMessageService;
    private readonly IRemoveProjectFromGroupNotificationService removeProjectFromGroupNotificationService = removeProjectFromGroupNotificationService;

    public async Task Handle(RemoveProjectFromGroupCommand command)
    {
        var project = await projectRepository.GetOne(command.ProjectId);

        if (project == null)
        {
            notificationMessageService.Create
            (
                "Project to remove not found!",
                "Remove Project from Group",
                NotificationType.Error
            );
            return;
        }

        project.GroupId = null;

        var result = await projectRepository.Edit(project);
        if (result)
        {
            removeProjectFromGroupNotificationService.Create(project.Id);
        }
    }
}

public class RemoveProjectFromGroupEventArgs(long productId) : EventArgs
{
    public long ProductId { get; } = productId;
}

public interface IRemoveProjectFromGroupNotificationService
{
    event EventHandler<RemoveProjectFromGroupEventArgs>? Notify;
    void Create(long projectId);
}

internal class RemoveProjectFromGroupNotificationService : IRemoveProjectFromGroupNotificationService
{
    private EventHandler<RemoveProjectFromGroupEventArgs>? _onNotifyOccured;

    public event EventHandler<RemoveProjectFromGroupEventArgs>? Notify
    {
        add { _onNotifyOccured += value; }
        remove { _onNotifyOccured -= value; }
    }

    public void Create(long projectId)
    {
        this._onNotifyOccured!.Invoke(this, new(projectId));
    }
}


