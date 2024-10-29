using ApplicationCore.Features.DevApps;
using ApplicationCore.Features.Git;
using ApplicationCore.Features.Groups;
using ApplicationCore.Features.Projects;
using Infrastructure.Models;
using Infrastructure.ViewModels;
using System.Windows;
using System.Windows.Input;
using UI.MainWindowx.Presenter;
using UI.Windows.Group;
namespace UI;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window, IMainWindowPresenter
{
    private readonly IAddDevAppService addDevAppService;
    private readonly IEditDevAppService? editDevAppService;
    private readonly IDeleteDevAppService? deleteDevAppService;
    private readonly IGetAllDevAppService? getAllDevAppService;
    private readonly IGetOneDevAppService? getOneDevAppService;
    private readonly IProjectFeaturesCreator? projectFeaturesCreator;
    private readonly IGroupFeaturesCreator? groupFeaturesCreator;
    private readonly IGetAllGroupService? getAllGroupService;
    private readonly IAddProjectService? addProjectService;
    private readonly IGetAllProjectService? getAllProjectService;
    private readonly IDeleteProjectService? deleteProjectService;
    private readonly IEditProjectService? editProjectService;
    private readonly ISearchProjectService? searchProductService;
    private readonly IOpenProjectFolderWindowService? openProjectFolderWindowService;
    private readonly IOpenProjectDevAppService? openProjectDevAppService;
    private readonly IGetCurrentGitBranchService? getCurrentGitBranchService;
    private readonly IRemoveProjectFromGroupService? removeProjectFromGroupService;
    private readonly IAddProjectToGroupService? addProjectToGroupService;
    private readonly ISortUpProjectService? sortUpProjectService;
    private readonly ISortDownProjectService? sortDownProjectService;
    private readonly MainWindowViewModel? mainWindowViewModel;
    private GroupModalWindow? groupModalWindow;
    private readonly List<Group> groups = [];

    public string DevAppFilePath { get; set; } = "";


    public event EventHandler? OpenDevApp;
    public event EventHandler? FetchDevAppsEvent;
    public IAddDevAppService? AddDevAppService
    {
        get { return this.addDevAppService; }
    }
    public MainWindowViewModel MainWindowViewModel { get; set; }

    public IGetAllDevAppService? GetAllDevAppService => this.getAllDevAppService;

    public MainWindow() => InitializeComponent();

    public MainWindow(
        IDevAppFeaturesCreator devAppFeaturesCreator,
        IProjectFeaturesCreator projectFeaturesCreator,
        IGitFeaturesCreator gitFeaturesCreator,
        IGroupFeaturesCreator groupFeaturesCreator
    )
    {
        #region Dev App Services
        this.addDevAppService = devAppFeaturesCreator.CreateAddDevAppService();
        this.editDevAppService = devAppFeaturesCreator.CreateEditDevAppService();
        this.deleteDevAppService = devAppFeaturesCreator.CreateDeleteDevAppService();
        this.getAllDevAppService = devAppFeaturesCreator.CreateGetAllDevAppService();
        this.getOneDevAppService = devAppFeaturesCreator.CreateGetOneDevAppService();
        #endregion
        #region Group Services
        this.getAllGroupService = groupFeaturesCreator.CreateGetAllGroupService();
        #endregion
        #region Project Services
        #region Commands
        this.addProjectService = projectFeaturesCreator.AddProjectServiceInstance;
        this.editProjectService = projectFeaturesCreator.CreateEditAddProjectService();
        this.deleteProjectService = projectFeaturesCreator.CreateDeleteAddProjectService();
        this.openProjectFolderWindowService = projectFeaturesCreator.CreateOpenProjectFolderWindowAppService();
        this.openProjectDevAppService = projectFeaturesCreator.CreateOpenProjectDevAppService();
        this.removeProjectFromGroupService = projectFeaturesCreator.CreateRemoveProjectFromGroupService();
        this.addProjectToGroupService = projectFeaturesCreator.CreateAddProjectToGroupService();
        this.sortUpProjectService = projectFeaturesCreator.CreateSortUpProjectService();
        this.sortDownProjectService = projectFeaturesCreator.CreateSortDownProjectService();
        #endregion
        #region Queries
        this.getAllProjectService = projectFeaturesCreator.CreateGetAllProjectService();
        this.searchProductService = projectFeaturesCreator.CreateSearchProjectService();
        this.getCurrentGitBranchService = gitFeaturesCreator.CreateGetCurrentGitBranchService();
        #endregion
        #endregion

        this.mainWindowViewModel = new MainWindowViewModel();
        InitializeComponent();
        var presenter = new MainWindowPresenter(this);
        DataContext = this.MainWindowViewModel;
    }



    private async void Window_Loaded(object sender, RoutedEventArgs e)
    {
        this.removeProjectFromGroupService!.Notify += RemoveProjectFromGroupNotificationService_Notify;
        this.addProjectToGroupService!.Notify += AddProjectToGroupService_Notify;
        await this.FetchProjects();
        this.FetchDevAppsEvent!.Invoke(this, EventArgs.Empty);
    }

    private async void AddProjectToGroupService_Notify(object? sender, EventArgs e)
    {
        await this.Search();
        SelectEditedItem();
        groupModalWindow!.Close();
    }

    private async void RemoveProjectFromGroupNotificationService_Notify(object? sender, RemoveProjectFromGroupEventArgs e)
    {
        await this.Search();
        SelectEditedItem();
        groupModalWindow!.Close();
    }

    private void VsCodePathOpenDialogButton_Click(object sender, RoutedEventArgs e)
    {
        OpenDevAppDialog();
    }

    private void ProjectPathsList_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {
        SelectProject();
    }

    private void btnOpenDialogProjectPath_Click(object sender, RoutedEventArgs e)
    {
        OpenProjectDialog();
    }
    private async void btnSaveProjectPath_Click(object sender, RoutedEventArgs e)
    {
        await SaveProject();
    }

    private void ProjectPathsList_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        OpenProject();
    }

    private async void btnDeleteIdePath_Click(object sender, RoutedEventArgs e)
    {
        await DeleteDevApp();
    }

    private void btnNewProjectPath_Click(object sender, RoutedEventArgs e)
    {
        this.mainWindowViewModel!.SelectedProjectPath = new();
        this.mainWindowViewModel!.SelectedIdePath = new();
    }

    private async void btnDeleteProjectPath_Click(object sender, RoutedEventArgs e)
    {
        await DeleteProject();
    }

    private async void mnuMoveUp_Click(object sender, RoutedEventArgs e)
    {
        await SortUpProject();
    }

    private async void mnuMoveDown_Click(object sender, RoutedEventArgs e)
    {
        await SortDownProject();
    }

    private async void TxtSearch_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
    {
        await Search();
    }

    private void MnuOpenFolderWindow_Click(object sender, RoutedEventArgs e)
    {
        this.openProjectFolderWindowService!.Handle
        (
            new() { Path = this.mainWindowViewModel!.SelectedProjectPath!.Path }
        );
    }

    private void txtSearch_KeyUp(object sender, KeyEventArgs e)
    {
        FocusOnListViewWhenArrowDown(e);
    }

    private void lvProjectPaths_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
    {
        OpenProjectWhenEnter(e);
    }

    private void mnuAddToGroup_Click(object sender, RoutedEventArgs e)
    {
        OpenAddToGroupDialog();
    }
}