using Infrastructure;

namespace ApplicationCore.Features.Projects;

public class GetAllProjectViewModel
{
    public IEnumerable<ProjectViewModel> Projects { get; init; } = [];
    public int Count { get { return this.Projects.Count(); } }
};

public interface IGetAllProjectService
{
    Task<GetAllProjectViewModel> GetAll();
}

public class GetAllProjectService(IProjectRepository projectRepository, IGitService gitService) : IGetAllProjectService
{
    private readonly IProjectRepository projectRepository = projectRepository;
    private readonly IGitService gitService = gitService;

    public async Task<GetAllProjectViewModel> GetAll()
    {
        var projects = await this.projectRepository.GetAll();

        return new()
        {
            Projects = projects.Select
            (
                (value, index) => new ProjectViewModel
                {

                    Id = value.Id,
                    Name = value.Name,
                    Path = value.Path,
                    IDEPathId = value.IDEPathId,
                    SortId = value.SortId,
                    Filename = value.Filename,
                    GroupId = value.GroupId,
                    Index = index + 1,
                    EnableMoveUp = index != 1,
                    EnableMoveDown = index != projects.Count(),
                    EnableAddToGroup = false,
                    GroupName = "",
                    CurrentGitBranch = $"Current Git Branch: {this.gitService.GetCurrentBranch(value.Path)}"
                }
            )
        };
    }
}

