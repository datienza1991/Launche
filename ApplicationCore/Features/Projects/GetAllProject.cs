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

internal class GetAllProjectService
(
    IProjectRepository projectRepository,
    IDevAppRepository devAppRepository,
    IGitService gitService,
    IGroupRepository groupRepository
)
    : IGetAllProjectService
{
    private readonly IProjectRepository projectRepository = projectRepository;
    private readonly IDevAppRepository devAppRepository = devAppRepository;
    private readonly IGitService gitService = gitService;
    private readonly IGroupRepository groupRepository = groupRepository;

    public async Task<GetAllProjectViewModel> Handle()
    {
        var projects = await this.projectRepository.GetAll();
        var devApps = await this.devAppRepository.GetAll();
        var groups = await this.groupRepository.GetAll();
        return new()
        {
            Projects = projects.Select
            (
                (value, index) =>
                {
                    var position = index + 1;
                    return new ProjectViewModel
                    {
                        Id = value.Id,
                        Name = value.Name,
                        Path = value.Path,
                        IDEPathId = value.IDEPathId,
                        SortId = value.SortId,
                        Filename = value.Filename,
                        GroupId = value.GroupId,
                        Index = position,
                        EnableMoveUp = position != 1,
                        EnableMoveDown = position != projects.Count(),
                        EnableAddToGroup = true,
                        GroupName = groups.FirstOrDefault(group => group.Id == value.GroupId)?.Name,
                        DevAppPath = devApps.First(devApp => devApp.Id == value.IDEPathId).Path,
                        CurrentGitBranch = $"Current Git Branch: {this.gitService.GetCurrentBranch(value.Path)}"
                    };
                }
            )
        };
    }
}

