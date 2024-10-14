namespace ApplicationCore.Features.Projects
{
    public class ProjectViewModel
    {
        public long Id { get; set; } = 0;
        public string Name { get; set; } = "";
        public string Path { get; set; } = "";
        public int IDEPathId { get; set; }
        public int SortId { get; init; }
        public string Filename { get; init; } = "";
        public long? GroupId { get; set; }
        public int Index { get; init; }
        public bool EnableMoveUp { get; init; }
        public bool EnableMoveDown { get; init; }
        public bool EnableAddToGroup { get; init; } = true;
        public bool EnabledGroupReset { get { return GroupId is not null; } }
        public string? GroupName { get; init; }
        public string CurrentGitBranch { get; set; } = "";
        public string FullPath { get { return $@"{Path}\{Filename}"; } }
        public bool HasFileName
        {
            get
            {
                return Filename is not null && Filename is not "";
            }
        }

        public string DevAppPath { get; set; } = string.Empty;
    }

    public static class ProjectViewModelExtension
    {
        public static ProjectViewModel Copy(this ProjectViewModel value)
        {
            if (value is null)
            {
                return new();
            }

            return new()
            {
                CurrentGitBranch = value.CurrentGitBranch,
                DevAppPath = value.DevAppPath,
                EnableAddToGroup = value.EnableAddToGroup,
                EnableMoveDown = value.EnableMoveDown,
                EnableMoveUp = value.EnableMoveUp,
                Filename = value.Filename,
                GroupId = value.GroupId,
                GroupName = value.GroupName,
                Id = value.Id,
                IDEPathId = value.IDEPathId,
                Index = value.Index,
                Name = value.Name,
                SortId = value.SortId,
                Path = value.Path,
            };
        }
    }
}
