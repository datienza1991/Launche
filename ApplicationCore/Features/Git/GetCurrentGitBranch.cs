using Infrastructure;

namespace ApplicationCore.Features.Git;

public class GetCurrentGitBranchQuery
{
    public string DirectoryPath { get; set; } = "";
}

public interface IGetCurrentGitBranchService
{
    string Handle(GetCurrentGitBranchQuery query);
}

internal class GetCurrentGitBranchService(IGitService gitService) : IGetCurrentGitBranchService
{
    private readonly IGitService gitService = gitService;

    public string Handle(GetCurrentGitBranchQuery query)
    {
        return gitService.GetCurrentBranch(query.DirectoryPath);
    }
}
