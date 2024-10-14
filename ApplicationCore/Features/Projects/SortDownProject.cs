namespace ApplicationCore.Features.Projects;

public class SortDownProjectCommand
{
    public int SortId { get; set; }
}

public interface ISortDownProjectService
{
    Task<bool> Handle(SortDownProjectCommand command);
}

internal class SortDownProjectService(IProjectRepository projectRepository) : ISortDownProjectService
{
    private readonly IProjectRepository projectRepository = projectRepository;

    public async Task<bool> Handle(SortDownProjectCommand command)
    {
        return await projectRepository.SortDown(command.SortId);
    }
}

