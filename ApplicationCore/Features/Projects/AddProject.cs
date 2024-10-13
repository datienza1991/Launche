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
            var result = await this.projectRepository.Add
            (
                new()
                {
                    Name = command.Name,
                    Path = command.Path,
                    IDEPathId = command.IDEPathId,
                    Filename = command.FileName
                }
            );

            if (result)
            {
                notificationMessageService.Create("New Project has been save!", "Add Project", NotificationType.Information);
            }

            return result;
        }
    }
}

