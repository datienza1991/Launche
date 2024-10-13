using ApplicationCore.Features.Projects;
using Infrastructure.Repositories;

namespace ApplicationCore.Features.Groups;

public interface IGroupFeaturesCreator
{
    IGetAllGroupService CreateGetAllGroupService();
}

internal class GroupFeaturesCreator(IGroupRepository groupRepository, IProjectRepository projectRepository) : IGroupFeaturesCreator
{
    private readonly IGroupRepository groupRepository = groupRepository;
    private readonly IProjectRepository projectRepository = projectRepository;

    public IGetAllGroupService CreateGetAllGroupService()
    {
        return new GetAllGroupService(groupRepository, projectRepository);
    }
}

