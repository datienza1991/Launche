using Infrastructure.Models;
using Infrastructure.Repositories;

namespace ApplicationCore.Features.DevApps
{
    public interface IDevAppCommand
    {
        Task<bool> Add(IDEPath param);
        Task<bool> Edit(IDEPath param);
        Task<bool> Delete(int id);
    }

    internal class DevAppCommand(IDevAppRepository devAppRepository) : IDevAppCommand
    {
        private readonly IDevAppRepository devAppRepository = devAppRepository;

        public async Task<bool> Add(IDEPath param)
        {
            return await devAppRepository.Add(param);
        }

        public async Task<bool> Delete(int id)
        {
            return await devAppRepository.Delete(id);
        }

        public async Task<bool> Edit(IDEPath param)
        {
            return await devAppRepository.Edit(param);
        }
    }
}
