using LibGit2Sharp;

namespace Infrastructure;

public interface IGitService
{
    string GetCurrentBranch(string projectPath);
}

public class GitService : IGitService
{
    public string GetCurrentBranch(string projectPath)
    {
        try
        {
            using var repo = new Repository(projectPath);

            if (repo == null)
            {
                return "";
            }

            var branch = repo.Branches.FirstOrDefault(branch => branch.IsCurrentRepositoryHead);

            return branch?.FriendlyName ?? "";
        }
        catch (RepositoryNotFoundException)
        {
            return "No Git Repository for this project!";
        }
        catch (Exception ex)
        {
            return $"Other errors thrown: {ex.Message}";
        }
    }
}

