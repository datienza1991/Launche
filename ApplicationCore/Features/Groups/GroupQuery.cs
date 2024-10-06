using Infrastructure.Models;

namespace ApplicationCore.Features.Groups
{
    public interface IGroupQuery
    {
        Task<Group> GetOne(int id);
        Task<List<Group>> GetAll();
    }
    public class GroupQuery(IGroupRepository groupRepository) : IGroupQuery
    {
        private readonly IGroupRepository groupRepository = groupRepository;

        public async Task<List<Group>> GetAll()
        {
            return await this.groupRepository.GetAll();
        }

        public async Task<Group> GetOne(int id)
        {
            return await this.groupRepository.GetOne(id);
        }
    }
}
