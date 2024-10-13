using ApplicationCore.Common;
using Infrastructure;
using Infrastructure.Repositories;

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
        IOpenProjectFolderWindowService CreateOpenProjectFolderWindowAppService();
        IOpenProjectDevAppService CreateOpenProjectDevAppService();
        IAddProjectToGroupService CreateAddProjectToGroupService();
        IRemoveProjectFromGroupService CreateRemoveProjectFromGroupService();
    }

    public class ProjectFeaturesCreator
    (
        IProjectRepository projectRepository,
        IDevAppRepository devAppRepository,
        IGitService gitService,
        INotificationMessageService notificationMessageService,
        IGroupRepository groupRepository
    )
        : IProjectFeaturesCreator
    {
        private readonly IProjectRepository projectRepository = projectRepository;
        private readonly IDevAppRepository devAppRepository = devAppRepository;
        private readonly IGitService gitService = gitService;
        private readonly INotificationMessageService notificationMessageService = notificationMessageService;

        public IAddProjectService CreateAddProjectService()
        {
            return new AddProjectService(projectRepository, notificationMessageService);
        }
        public IEditProjectService CreateEditAddProjectService()
        {
            return new EditProjectService(projectRepository, notificationMessageService);
        }
        public IDeleteProjectService CreateDeleteAddProjectService()
        {
            return new DeleteProjectService(projectRepository);
        }

        public IGetAllProjectService CreateGetAllProjectService()
        {
            return new GetAllProjectService(projectRepository, devAppRepository, gitService);
        }

        public IGetLastProjectService CreateGetLastProjectService()
        {
            return new GetLastProject(projectRepository);
        }

        public ISearchProjectService CreateSearchProjectService()
        {
            return new SearchProjectService(projectRepository);
        }

        public IOpenProjectFolderWindowService CreateOpenProjectFolderWindowAppService()
        {
            return new OpenProjectFolderWindowService(notificationMessageService);
        }

        public IOpenProjectDevAppService CreateOpenProjectDevAppService()
        {
            return new OpenProjectDevAppService(notificationMessageService);
        }

        public IAddProjectToGroupService CreateAddProjectToGroupService()
        {
            return new AddProjectToGroupService(groupRepository, projectRepository, notificationMessageService);
        }

        public IRemoveProjectFromGroupService CreateRemoveProjectFromGroupService()
        {
            return new RemoveProjectFromGroupService(projectRepository, notificationMessageService);
        }
    }
}
