namespace ApplicationCore.Features.Basic.Project;

public class ProjectDetail
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Path { get; set; } = string.Empty;
    public int IDEPathId { get; set; }
    public int SortId { get; set; }
    public string Filename { get; set; } = string.Empty;
    public int? GroupId { get; internal set; }
}

