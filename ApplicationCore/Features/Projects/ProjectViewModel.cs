namespace ApplicationCore.Features.Projects
{
    public class ProjectViewModel
    {
        public long Id { get; set; }
        public string Name { get; init; } = "";
        public string Path { get; init; } = "";
        public int IDEPathId { get; set; }
        public int SortId { get; init; }
        public string Filename { get; init; } = "";
        public int? GroupId { get; set; }
        public int Index { get; init; }
        public bool EnableMoveUp { get; init; }
        public bool EnableMoveDown { get; init; }
        public bool EnableAddToGroup { get; init; }
        public string? GroupName { get; init; }
        public string CurrentGitBranch { get; init; } = "";
    }
}
