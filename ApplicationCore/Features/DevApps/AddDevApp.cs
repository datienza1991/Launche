using Infrastructure.Repositories;

namespace ApplicationCore.Features.DevApps;

public class AddDevAppCommand
{
    public string Path { get; set; } = string.Empty;
}

public interface IAddDevAppService
{
    Task<bool> Add(AddDevAppCommand command);
}

internal class AddDevAppService(IDevAppRepository devAppRepository) : IAddDevAppService
{
    private readonly IDevAppRepository devAppRepository = devAppRepository;

    public async Task<bool> Add(AddDevAppCommand command)
    {
        return await devAppRepository.Add(new() { Path = command.Path });
    }
}

