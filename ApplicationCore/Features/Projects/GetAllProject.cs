using Infrastructure;
using Infrastructure.Repositories;

namespace ApplicationCore.Features.Projects;

public class GetAllProjectViewModel
{
    public IEnumerable<ProjectViewModel> Projects { get; init; } = [];
    public int Count { get { return this.Projects.Count(); } }
};

public interface IGetAllProjectService
{
    Task<GetAllProjectViewModel> Handle();
}

internal class GetAllProjectService(IProjectRepository projectRepository, IDevAppRepository devAppRepository, IGitService gitService) : IGetAllProjectService
{
    private readonly IProjectRepository projectRepository = projectRepository;
    private readonly IDevAppRepository devAppRepository = devAppRepository;
    private readonly IGitService gitService = gitService;

    public async Task<GetAllProjectViewModel> Handle()
    {
        var projects = await this.projectRepository.GetAll();
        var devApps = await this.devAppRepository.GetAll();

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
                    EnableAddToGroup = true,
                    GroupName = "",
                    DevAppPath = devApps.First(devApp => devApp.Id == value.IDEPathId).Path,
                    CurrentGitBranch = $"Current Git Branch: {this.gitService.GetCurrentBranch(value.Path)}"
                }
            )
        };
    }
}

