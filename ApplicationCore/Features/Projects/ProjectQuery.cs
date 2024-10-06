using Infrastructure.Models;

namespace ApplicationCore.Features.Projects
{
    public interface IProjectQuery
    {
        Task<Project> GetLast();
        Task<IEnumerable<ProjectPathViewModel>> GetAll();
    }

    public class ProjectQuery(IProjectRepository projectRepository) : IProjectQuery
    {
        private readonly IProjectRepository projectRepository = projectRepository;

        public async Task<IEnumerable<ProjectPathViewModel>> GetAll()
        {
            var projects = await this.projectRepository.GetAll();
            return projects.Select
        (
            (value, index) => new ProjectPathViewModel
            {
                Index = index + 1,
                Id = value.Id,
                Name = value.Name,
                Path = value.Path,
                IDEPathId = value.IDEPathId,
                SortId = value.SortId,
                EnableMoveUp = index != 1,
                EnableMoveDown = index != projects.Count(),
                Filename = value.Filename,
                GroupId = value.GroupId,
                GroupName = ""
            }
        );
        }

        public async Task<Project> GetLast()
        {
            return await this.projectRepository.GetLast();
        }
    }
}
