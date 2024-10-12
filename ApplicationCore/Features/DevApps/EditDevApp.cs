using Infrastructure.Repositories;

namespace ApplicationCore.Features.DevApps
{
    public class EditDevAppCommand
    {
        public string Path { get; set; } = string.Empty;
        public int Id { get; internal set; }
    }

    public interface IEditDevAppService
    {
        Task<bool> Edit(EditDevAppCommand command);
    }

    internal class EditDevAppService(IDevAppRepository devAppRepository) : IEditDevAppService
    {
        private readonly IDevAppRepository devAppRepository = devAppRepository;

        public async Task<bool> Edit(EditDevAppCommand command)
        {
            return await devAppRepository.Edit(new() { Path = command.Path, Id = command.Id });
        }
    }
}
