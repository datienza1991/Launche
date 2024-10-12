namespace ApplicationCore.Features.Projects;

public interface IDeleteProjectService
{
    Task<bool> Delete(long id);
}
internal class DeleteProjectService(IProjectRepository projectRepository) : IDeleteProjectService
{
    private readonly IProjectRepository projectRepository = projectRepository;

    public async Task<bool> Delete(long id)
    {
        return await this.projectRepository.Delete(id);
    }
}

