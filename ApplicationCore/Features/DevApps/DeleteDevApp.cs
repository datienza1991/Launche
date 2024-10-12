using Infrastructure.Repositories;

namespace ApplicationCore.Features.DevApps;

public class DeleteDevAppCommand
{
    public long Id { get; init; }
}

public interface IDeleteDevAppService
{
    Task<bool> Delete(DeleteDevAppCommand command);
}

internal class DeleteDevAppService(IDevAppRepository devAppRepository) : IDeleteDevAppService
{
    private readonly IDevAppRepository devAppRepository = devAppRepository;

    public async Task<bool> Delete(DeleteDevAppCommand command)
    {
        return await devAppRepository.Delete(command.Id);
    }
}

