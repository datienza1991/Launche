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

    internal class EditProjectService(IProjectRepository projectRepository) : IEditProjectService
    {
        private readonly IProjectRepository projectRepository = projectRepository;

        public async Task<bool> Edit(EditProjectCommand command)
        {
            return await this.projectRepository.Edit
            (
                new()
                {
                    Id = command.Id,
                    Name = command.Name,
                    Path = command.Path,
                    IDEPathId = command.IDEPathId,
                    Filename = command.FileName,
                }
            );
        }
    }
}
