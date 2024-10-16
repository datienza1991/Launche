﻿namespace Infrastructure.Models;

public class Project
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Path { get; set; } = string.Empty;
    public int IDEPathId { get; set; }
    public int SortId { get; set; }
    public string Filename { get; set; } = string.Empty;
    public long? GroupId { get; set; }
}