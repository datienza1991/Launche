using Infrastructure;

namespace ApplicationCore.Features.Git
{
    public interface IGitFeaturesCreator
    {
        IGetCurrentGitBranchService CreateGetCurrentGitBranchService();
    }

    internal class GitFeaturesCreator(IGitService gitService) : IGitFeaturesCreator
    {
        private readonly IGitService gitService = gitService;

        public IGetCurrentGitBranchService CreateGetCurrentGitBranchService()
        {
            return new GetCurrentGitBranchService(gitService);
        }
    }
}
