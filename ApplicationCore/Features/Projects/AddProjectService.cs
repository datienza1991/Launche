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
    public class AddProjectService(IProjectRepository projectRepository) : IAddProjectService
    {
        private readonly IProjectRepository projectRepository = projectRepository;

        public async Task<bool> AddAsync(AddProjectCommand command)
        {
            return await this.projectRepository.Add
            (
                new()
                {
                    Name = command.Name,
                    Path = command.Path,
                    IDEPathId = command.IDEPathId,
                    Filename = command.FileName
                }
            );
        }
    }
}

