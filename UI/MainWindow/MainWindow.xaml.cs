using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ApplicationCore.Features.DevApps;
using ApplicationCore.Features.Git;
using ApplicationCore.Features.Groups;
using ApplicationCore.Features.Projects;
using Infrastructure.Models;
using Infrastructure.ViewModels;
using UI.MainWindowx.Presenter;
using UI.Windows.Group;

namespace UI;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window, IMainWindowView
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
    private readonly ISearchProjectService? searchProjectService;
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
    public event EventHandler DeleteDevAppEvent;
    public event EventHandler NewProjectEvent;
    public event EventHandler OpenProjectDialog;
    public event EventHandler DeleteProjectEvent;
    public event EventHandler SearchProjectEvent;
    public event EventHandler SortUpProjectEvent;
    public event EventHandler SelectProjectEvent;
    public event EventHandler SortDownProjectEvent;
    public event EventHandler OpenAddToGroupModalWindowEvent;
    public event EventHandler SaveProjectEvent;
    public event EventHandler OpenProjectDevAppEvent;
    public event EventHandler OpenProjectFolderWindowEvent;
    public event EventHandler FocusOnListViewEvent;

    public IAddDevAppService? AddDevAppService
    {
        get { return this.addDevAppService; }
    }
    public MainWindowViewModel MainWindowViewModel { get; set; }

    public IGetAllDevAppService? GetAllDevAppService => this.getAllDevAppService;

    public IDeleteDevAppService? DeleteDevAppService => this.deleteDevAppService;

    public IDeleteProjectService? DeleteProjectService => this.deleteProjectService;

    public ISearchProjectService? SearchProjectService => this.searchProjectService;

    public ISortUpProjectService? SortUpProjectService => this.sortUpProjectService;

    public ListView ProjectPathsListView => this.lvProjectPaths;

    public IGetCurrentGitBranchService? GetCurrentGitBranchService => getCurrentGitBranchService;

    public ISortDownProjectService? SortDownProjectService => sortDownProjectService;

    public IGetAllGroupService? GetAllGroupService => getAllGroupService;

    public IAddProjectToGroupService? AddProjectToGroupService => addProjectToGroupService;

    public IRemoveProjectFromGroupService? RemoveProjectFromGroupService =>
        removeProjectFromGroupService;

    public IAddProjectService? AddProjectService => addProjectService;

    public IEditProjectService? EditProjectService => editProjectService;

    public IOpenProjectDevAppService? OpenProjectDevAppService => openProjectDevAppService;

    public IOpenProjectFolderWindowService? OpenProjectFolderWindowService =>
        openProjectFolderWindowService;

    public ProjectViewModel SelectedProject => (ProjectViewModel)lvProjectPaths.SelectedItem;

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
        this.openProjectFolderWindowService =
            projectFeaturesCreator.CreateOpenProjectFolderWindowAppService();
        this.openProjectDevAppService = projectFeaturesCreator.CreateOpenProjectDevAppService();
        this.removeProjectFromGroupService =
            projectFeaturesCreator.CreateRemoveProjectFromGroupService();
        this.addProjectToGroupService = projectFeaturesCreator.CreateAddProjectToGroupService();
        this.sortUpProjectService = projectFeaturesCreator.CreateSortUpProjectService();
        this.sortDownProjectService = projectFeaturesCreator.CreateSortDownProjectService();
        #endregion
        #region Queries
        this.getAllProjectService = projectFeaturesCreator.CreateGetAllProjectService();
        this.searchProjectService = projectFeaturesCreator.CreateSearchProjectService();
        this.getCurrentGitBranchService = gitFeaturesCreator.CreateGetCurrentGitBranchService();
        #endregion
        #endregion

        this.mainWindowViewModel = new MainWindowViewModel();
        InitializeComponent();
        var presenter = new MainWindowPresenter(this);
        DataContext = this.MainWindowViewModel;
    }

    public void FocusOnListViewWhenArrowDown()
    {
        if (lvProjectPaths.Items.Count == 0)
        {
            return;
        }

        lvProjectPaths.Focus();
        var item = lvProjectPaths.Items[0];
        lvProjectPaths.SelectedItem = item;
        ListViewItem? firstListViewItem =
            lvProjectPaths.ItemContainerGenerator.ContainerFromIndex(0) as ListViewItem;
        firstListViewItem?.Focus();
    }

    public void SelectNewlyAddedItem()
    {
        lvProjectPaths.SelectedItem = lvProjectPaths.Items[^1];
        lvProjectPaths.ScrollIntoView(this.lvProjectPaths.SelectedItem);
    }

    public void SelectEditedItem()
    {
        lvProjectPaths.SelectedItem = lvProjectPaths
            .Items.SourceCollection.Cast<ProjectViewModel>()
            .FirstOrDefault(projectPathsViewModel =>
                projectPathsViewModel.Id == this.MainWindowViewModel?.SelectedProjectPath?.Id
            );

        lvProjectPaths.ScrollIntoView(this.lvProjectPaths.SelectedItem);
    }

    private async void Window_Loaded(object sender, RoutedEventArgs e)
    {
        this.SearchProjectEvent.Invoke(this, EventArgs.Empty);
        this.FetchDevAppsEvent!.Invoke(this, EventArgs.Empty);
    }

    private void VsCodePathOpenDialogButton_Click(object sender, RoutedEventArgs e)
    {
        this.OpenDevApp!.Invoke(this, EventArgs.Empty);
    }

    private void ProjectPathsList_SelectionChanged(
        object sender,
        System.Windows.Controls.SelectionChangedEventArgs e
    )
    {
        if (lvProjectPaths.SelectedIndex == -1)
        {
            return;
        }

        this.SelectProjectEvent.Invoke(this, EventArgs.Empty);
    }

    private void btnOpenDialogProjectPath_Click(object sender, RoutedEventArgs e)
    {
        this.OpenProjectDialog.Invoke(this, EventArgs.Empty);
    }

    private void btnSaveProjectPath_Click(object sender, RoutedEventArgs e)
    {
        SaveProjectEvent.Invoke(this, EventArgs.Empty);
    }

    private void ProjectPathsList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        OpenProjectDevAppEvent.Invoke(this, EventArgs.Empty);
    }

    private void btnDeleteIdePath_Click(object sender, RoutedEventArgs e)
    {
        this.DeleteDevAppEvent.Invoke(this, EventArgs.Empty);
    }

    private void btnNewProjectPath_Click(object sender, RoutedEventArgs e)
    {
        this.NewProjectEvent.Invoke(this, EventArgs.Empty);
    }

    private void btnDeleteProjectPath_Click(object sender, RoutedEventArgs e)
    {
        this.DeleteProjectEvent.Invoke(this, EventArgs.Empty);
    }

    private void mnuMoveUp_Click(object sender, RoutedEventArgs e)
    {
        this.SortUpProjectEvent.Invoke(this, EventArgs.Empty);
    }

    private void mnuMoveDown_Click(object sender, RoutedEventArgs e)
    {
        this.SortDownProjectEvent.Invoke(this, EventArgs.Empty);
    }

    private void TxtSearch_TextChanged(
        object sender,
        System.Windows.Controls.TextChangedEventArgs e
    )
    {
        this.SearchProjectEvent.Invoke(this, EventArgs.Empty);
    }

    private void MnuOpenFolderWindow_Click(object sender, RoutedEventArgs e)
    {
        this.OpenProjectFolderWindowEvent.Invoke(this, EventArgs.Empty);
    }

    private void txtSearch_KeyUp(object sender, KeyEventArgs e)
    {
        if (e.Key != Key.Down)
        {
            return;
        }

        FocusOnListViewEvent.Invoke(this, EventArgs.Empty);
    }

    private void lvProjectPaths_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
    {
        if (e.Key != Key.Enter)
        {
            return;
        }

        this.OpenProjectDevAppEvent.Invoke(this, EventArgs.Empty);
    }

    private void mnuAddToGroup_Click(object sender, RoutedEventArgs e)
    {
        OpenAddToGroupModalWindowEvent.Invoke(this, EventArgs.Empty);
    }
}
