using Infrastructure.Models;

namespace ApplicationCore.Features.Projects
{
    public interface IProjectCommand
    {
        Task<bool> Add(Project param);
        Task<bool> Edit(Project param);
        Task<bool> Delete(int id);
    }
    public class ProjectCommand(IProjectRepository projectRepository) : IProjectCommand
    {
        private readonly IProjectRepository projectRepository = projectRepository;

        public async Task<bool> Add(Project param)
        {
            return await this.projectRepository.Add(param);
        }

        public async Task<bool> Delete(int id)
        {
            return await this.projectRepository.Delete(id);
        }

        public async Task<bool> Edit(Project param)
        {
            return await this.projectRepository.Edit(param);
        }
    }
}
