namespace ApplicationCore.Features.Projects;

public class SortUpProjectCommand
{
    public int SortId { get; set; }
}

public interface ISortUpProjectService
{
    Task<bool> Handle(SortUpProjectCommand command);
}

internal class SortUpProjectService(IProjectRepository projectRepository) : ISortUpProjectService
{
    private readonly IProjectRepository projectRepository = projectRepository;

    public async Task<bool> Handle(SortUpProjectCommand command)
    {
        return await projectRepository.SortUp(command.SortId);
    }
}

