namespace ApplicationCore.Features.Projects;

public interface IDeleteProjectService
{
    Task<bool> HandleAsync(long id);
}

public class DeleteProjectService(IProjectRepository projectRepository) : IDeleteProjectService
{
    private readonly IProjectRepository projectRepository = projectRepository;

    public async Task<bool> HandleAsync(long id)
    {
        return await this.projectRepository.Delete(id);
    }
}
