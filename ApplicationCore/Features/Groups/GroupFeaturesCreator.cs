using Infrastructure.Repositories;

namespace ApplicationCore.Features.Groups;

public interface IGroupFeaturesCreator
{
    IGetAllGroupService CreateGetAllGroupService();
}

internal class GroupFeaturesCreator(IGroupRepository groupRepository) : IGroupFeaturesCreator
{
    private readonly IGroupRepository groupRepository = groupRepository;

    public IGetAllGroupService CreateGetAllGroupService()
    {
        return new GetAllGroupService(groupRepository);
    }
}

