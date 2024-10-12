using Infrastructure.Repositories;
using Infrastructure.ViewModels;

namespace ApplicationCore.Features.DevApps
{
    public class GetOneDevAppQuery
    {
        public int Id { get; init; }
    }

    public interface IGetOneDevAppService
    {
        Task<IDEPathViewModel> GetOneAsync(GetOneDevAppQuery query);
    }

    public class GetOneDevAppService(IDevAppRepository devAppRepository) : IGetOneDevAppService
    {
        public async Task<IDEPathViewModel> GetOneAsync(GetOneDevAppQuery query)
        {
            var devApp = await devAppRepository.GetById(query.Id);

            return new() { Id = devApp.Id, Path = devApp.Path };
        }
    }
}
