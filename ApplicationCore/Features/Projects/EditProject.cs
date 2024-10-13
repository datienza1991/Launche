using ApplicationCore.Common;

namespace ApplicationCore.Features.Projects
{
    public record EditProjectCommand
    (
        long Id,
        string Name,
        string Path,
        int IDEPathId,
        string FileName
    );

    public interface IEditProjectService
    {
        Task<bool> Edit(EditProjectCommand param);
    }

    internal class EditProjectService(IProjectRepository projectRepository, INotificationMessageService notificationMessageService) : IEditProjectService
    {
        private readonly IProjectRepository projectRepository = projectRepository;

        public async Task<bool> Edit(EditProjectCommand command)
        {
            var project = await projectRepository.GetOne(command.Id);

            if (project == null)
            {
                notificationMessageService.Create
                (
                    "Project to edit not found!",
                    "Edit Project",
                    NotificationType.Error
                );
                return false;
            }

            project.Name = command.Name;
            project.Path = command.Path;
            project.IDEPathId = command.IDEPathId;
            project.Filename = command.FileName;

            var result = await this.projectRepository.Edit(project);
            if (result)
            {
                notificationMessageService.Create("Record has been updated!", "Edit Project", NotificationType.Success);
            }

            return result;
        }
    }
}
