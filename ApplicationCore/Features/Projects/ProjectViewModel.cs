namespace ApplicationCore.Features.Projects
{
    public class ProjectViewModel
    {
        public long Id { get; set; } = 0;
        public string Name { get; init; } = "";
        public string Path { get; init; } = "";
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
}
