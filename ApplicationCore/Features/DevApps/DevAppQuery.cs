using Infrastructure.Models;
using Infrastructure.Repositories;

namespace ApplicationCore.Features.DevApps
{
    public interface IDevAppQuery
    {
        Task<IDEPath> GetById(int id);
        Task<IEnumerable<IDEPath>> GetAll();
    }

    public class DevAppQuery(IDevAppRepository devAppRepository) : IDevAppQuery
    {
        public async Task<IEnumerable<IDEPath>> GetAll()
        {
            return await devAppRepository.GetAll();
        }

        public async Task<IDEPath> GetById(int id)
        {
            return await devAppRepository.GetById(id);
        }
    }
}
