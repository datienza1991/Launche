using ApplicationCore.Common;

namespace ApplicationCore.Features.Projects
{
    public record AddProjectCommand
    (
        string Name,
        string Path,
        int IDEPathId,
        string FileName
    );

    public interface IAddProjectService
    {
        Task<bool> AddAsync(AddProjectCommand command);
    }
    internal class AddProjectService(IProjectRepository projectRepository, INotificationMessageService notificationMessageService) : IAddProjectService
    {
        private readonly IProjectRepository projectRepository = projectRepository;
        private readonly INotificationMessageService notificationMessageService = notificationMessageService;

        public async Task<bool> AddAsync(AddProjectCommand command)
        {
            if (command.Name is null || command.Name is "")
            {
                notificationMessageService.Create("Project Name must be provided", "Add Project", NotificationType.Error);
                return false;
            }

            if (command.Path is null || command.Path is "")
            {
                notificationMessageService.Create("Project Path must be provided", "Add Project", NotificationType.Error);
                return false;
            }

            if (command.IDEPathId is 0)
            {
                notificationMessageService.Create("Project Dev App must be provided", "Add Project", NotificationType.Error);
                return false;
            }

            var projects = await projectRepository.GetAll();
            var lastSortId = projects.LastOrDefault()?.SortId ?? 0;
            var result = await this.projectRepository.Add
            (
                new()
                {
                    Name = command.Name,
                    Path = command.Path,
                    IDEPathId = command.IDEPathId,
                    Filename = command.FileName,
                    SortId = lastSortId + 1
                }
            ); ;

            if (result)
            {
                notificationMessageService.Create("New Project has been save!", "Add Project", NotificationType.Information);
            }

            return result;
        }
    }
}

