using Infrastructure;

namespace ApplicationCore.Features.Projects
{
    public interface IProjectFeaturesCreator
    {
        IAddProjectService CreateAddProjectService();
        IEditProjectService CreateEditAddProjectService();
        IDeleteProjectService CreateDeleteAddProjectService();
        IGetAllProjectService CreateGetAllProjectService();
        IGetLastProjectService CreateGetLastProjectService();
        ISearchProjectService CreateSearchProjectService();
    }

    public class ProjectFeaturesCreator(IProjectRepository projectRepository, IGitService gitService) : IProjectFeaturesCreator
    {
        private readonly IProjectRepository projectRepository = projectRepository;
        private readonly IGitService gitService = gitService;

        public IAddProjectService CreateAddProjectService()
        {
            return new AddProjectService(projectRepository);
        }
        public IEditProjectService CreateEditAddProjectService()
        {
            return new EditProjectService(projectRepository);
        }
        public IDeleteProjectService CreateDeleteAddProjectService()
        {
            return new DeleteProjectService(projectRepository);
        }

        public IGetAllProjectService CreateGetAllProjectService()
        {
            return new GetAllProjectService(projectRepository, gitService);
        }

        public IGetLastProjectService CreateGetLastProjectService()
        {
            return new GetLastProject(projectRepository);
        }

        public ISearchProjectService CreateSearchProjectService()
        {
            return new SearchProjectService(projectRepository);
        }
    }
}
