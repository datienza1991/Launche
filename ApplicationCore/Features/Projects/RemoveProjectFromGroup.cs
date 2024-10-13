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
    INotificationMessageService notificationMessageService
)
    : IRemoveProjectFromGroupService
{
    private readonly IProjectRepository projectRepository = projectRepository;
    private readonly INotificationMessageService notificationMessageService = notificationMessageService;

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

        await projectRepository.Edit(project);
    }
}

