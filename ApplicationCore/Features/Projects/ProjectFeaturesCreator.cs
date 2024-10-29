using ApplicationCore.Common;
using Infrastructure;
using Infrastructure.Repositories;

namespace ApplicationCore.Features.Projects
{
    public interface IProjectFeaturesCreator
    {
        IAddProjectService AddProjectServiceInstance { get; }
        IEditProjectService CreateEditAddProjectService();
        IDeleteProjectService CreateDeleteAddProjectService();
        IGetAllProjectService CreateGetAllProjectService();
        IGetLastProjectService CreateGetLastProjectService();
        ISearchProjectService CreateSearchProjectService();
        IOpenProjectFolderWindowService CreateOpenProjectFolderWindowAppService();
        IOpenProjectDevAppService CreateOpenProjectDevAppService();
        IAddProjectToGroupService CreateAddProjectToGroupService();
        IRemoveProjectFromGroupService CreateRemoveProjectFromGroupService();
        ISortUpProjectService CreateSortUpProjectService();
        ISortDownProjectService CreateSortDownProjectService();
    }

    public class ProjectFeaturesCreator : IProjectFeaturesCreator
    {
        private readonly IProjectRepository projectRepository;
        private readonly IDevAppRepository devAppRepository;
        private readonly IGitService gitService;
        private readonly INotificationMessageService notificationMessageService;
        private readonly IGroupRepository groupRepository;
        private readonly IAddProjectService _addProjectServiceInstance;

        public ProjectFeaturesCreator(
            IProjectRepository projectRepository,
            IDevAppRepository devAppRepository,
            IGitService gitService,
            INotificationMessageService notificationMessageService,
            IGroupRepository groupRepository
        )
        {
            this.projectRepository = projectRepository;
            this.devAppRepository = devAppRepository;
            this.gitService = gitService;
            this.notificationMessageService = notificationMessageService;
            this.groupRepository = groupRepository;
            this._addProjectServiceInstance ??= new AddProjectService(projectRepository, notificationMessageService);

        }

        public IAddProjectService AddProjectServiceInstance { get { return _addProjectServiceInstance; } }

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
            return new GetAllProjectService
            (
                projectRepository,
                devAppRepository,
                gitService,
                groupRepository
            );
        }

        public IGetLastProjectService CreateGetLastProjectService()
        {
            return new GetLastProject(projectRepository);
        }

        public ISearchProjectService CreateSearchProjectService()
        {
            return new SearchProjectService(projectRepository, groupRepository, devAppRepository);
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

        public ISortUpProjectService CreateSortUpProjectService()
        {
            return new SortUpProjectService(projectRepository);
        }

        public ISortDownProjectService CreateSortDownProjectService()
        {
            return new SortDownProjectService(projectRepository);
        }
    }
}
