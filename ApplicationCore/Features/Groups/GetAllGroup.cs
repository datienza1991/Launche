using ApplicationCore.Features.Projects;
using Infrastructure.Repositories;

namespace ApplicationCore.Features.Groups;

public class GetAllGroupViewModel
{
    public bool EnabledReset { get; set; } = true;
    public IEnumerable<GroupViewModel> Groups { get; init; } = [];
}

public class GetAllGroupQuery
{
    public long ProjectId { get; set; }
}
public interface IGetAllGroupService
{
    Task<GetAllGroupViewModel> Handle(GetAllGroupQuery query);
}

internal class GetAllGroupService(IGroupRepository groupRepository, IProjectRepository projectRepository) : IGetAllGroupService
{
    private readonly IGroupRepository groupRepository = groupRepository;
    private readonly IProjectRepository projectRepository = projectRepository;

    public async Task<GetAllGroupViewModel> Handle(GetAllGroupQuery query)
    {
        var groups = await groupRepository.GetAll();
        var project = await projectRepository.GetOne(query.ProjectId);

        return new()
        {
            EnabledReset = project.GroupId != null,
            Groups = groups.Select
            (
                group => new GroupViewModel() { Id = group.Id, Name = group.Name }
            )
        };
    }
}


