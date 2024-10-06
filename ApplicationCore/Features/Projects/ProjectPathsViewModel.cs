namespace ApplicationCore.Features.Projects
{
    public class ProjectPathViewModel : Infrastructure.Models.Project
    {
        public int Index { get; set; }
        public bool EnableMoveUp { get; set; }
        public bool EnableMoveDown { get; set; }
        public bool EnableAddToGroup { get; set; }
        public string? GroupName { get; set; }
        public string CurrentGitBranch { get; set; } = string.Empty;
    }

    public class ProjectPathTransformer
    {
        private string _currentGitBranch = "";

        public string CurrentGitBranch { get { return $"Current Git Branch: {_currentGitBranch}"; } set { _currentGitBranch = value; } }
        public static ProjectPathViewModel Transform(Infrastructure.Models.Project from, string repoName)
        {
            return new()
            {
                Id = from.Id,
                IDEPathId = from.IDEPathId,
                Name = from.Name,
                Path = from.Path,
                SortId = from.SortId,
                Filename = from.Filename,
                CurrentGitBranch = repoName,
                GroupId = from.GroupId,
            };
        }

        public static Infrastructure.Models.Project Transform(ProjectPathViewModel from)
        {
            return new()
            {
                Id = from.Id,
                IDEPathId = from.IDEPathId,
                Name = from.Name,
                Path = from.Path,
                Filename = from.Filename,
                GroupId = from.GroupId,
            };
        }
    }


}
