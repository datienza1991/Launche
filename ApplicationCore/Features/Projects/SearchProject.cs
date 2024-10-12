﻿namespace ApplicationCore.Features.Projects;

public class SearchProjectViewModel
{
    public IEnumerable<ProjectViewModel> Projects { get; set; } = [];
    public bool EnableAddNewProject { get; internal set; }
}

public class SearchProjectQuery
{
    public string Search { get; set; } = "";
}

public interface ISearchProjectService
{
    Task<SearchProjectViewModel> Handle(SearchProjectQuery query);
}

internal class SearchProjectService(IProjectRepository projectRepository) : ISearchProjectService
{
    private readonly IProjectRepository projectRepository = projectRepository;

    public async Task<SearchProjectViewModel> Handle(SearchProjectQuery query)
    {
        var projects = await projectRepository.GetAll();
        var filteredPaths = projects.Where(projectPath => projectPath.Name.ToLower().Contains(query.Search.ToLower()));
        SearchProjectViewModel vm = new();

        if (query.Search is not "")
        {
            vm.EnableAddNewProject = false;
            vm.Projects = filteredPaths.Select
            (
                (project, index) =>
                {
                    return new ProjectViewModel
                    {
                        EnableMoveDown = false,
                        EnableMoveUp = false,
                        Filename = project.Filename,
                        Id = project.Id,
                        IDEPathId = project.IDEPathId,
                        Index = index + 1,
                        Name = project.Name,
                        Path = project.Path,
                        SortId = project.SortId,
                        GroupId = project.GroupId,
                        GroupName = ""
                    };
                }
            );
        }
        else
        {
            vm.EnableAddNewProject = true;
            vm.Projects = filteredPaths.Select
            (
                (project, index) => new ProjectViewModel
                {
                    EnableMoveUp = index != 1,
                    EnableMoveDown = index != projects.Count(),
                    Filename = project.Filename,
                    Id = project.Id,
                    IDEPathId = project.IDEPathId,
                    Index = index + 1,
                    Name = project.Name,
                    Path = project.Path,
                    SortId = project.SortId,
                    GroupId = project.GroupId,
                    GroupName = ""
                }
            );
        }

        return vm;
    }
}

