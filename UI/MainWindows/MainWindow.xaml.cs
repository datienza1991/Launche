using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ApplicationCore.Features.DevApps;
using ApplicationCore.Features.Git;
using ApplicationCore.Features.Groups;
using ApplicationCore.Features.Projects;
using Infrastructure.Models;
using Infrastructure.ViewModels;
using Microsoft.Win32;
using UI.Windows.Group;

namespace UI.MainWindows;

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
    private GroupModalWindow? groupModalWindow;
    private readonly List<Group> groups = [];

    public string DevAppFilePath { get; set; } = "";

    public event EventHandler? AddNewDevAppEvent;
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
        get { return addDevAppService; }
    }
    public MainWindowViewModel MainWindowViewModel { get; set; }

    public IGetAllDevAppService? GetAllDevAppService => getAllDevAppService;

    public IDeleteDevAppService? DeleteDevAppService => deleteDevAppService;

    public IDeleteProjectService? DeleteProjectService => deleteProjectService;

    public ISearchProjectService? SearchProjectService => searchProjectService;

    public ISortUpProjectService? SortUpProjectService => sortUpProjectService;

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
        addDevAppService = devAppFeaturesCreator.CreateAddDevAppService();
        editDevAppService = devAppFeaturesCreator.CreateEditDevAppService();
        deleteDevAppService = devAppFeaturesCreator.CreateDeleteDevAppService();
        getAllDevAppService = devAppFeaturesCreator.CreateGetAllDevAppService();
        getOneDevAppService = devAppFeaturesCreator.CreateGetOneDevAppService();
        #endregion
        #region Group Services
        getAllGroupService = groupFeaturesCreator.CreateGetAllGroupService();
        #endregion
        #region Project Services
        #region Commands
        addProjectService = projectFeaturesCreator.AddProjectServiceInstance;
        editProjectService = projectFeaturesCreator.CreateEditAddProjectService();
        deleteProjectService = projectFeaturesCreator.CreateDeleteAddProjectService();
        openProjectFolderWindowService =
            projectFeaturesCreator.CreateOpenProjectFolderWindowAppService();
        openProjectDevAppService = projectFeaturesCreator.CreateOpenProjectDevAppService();
        removeProjectFromGroupService =
            projectFeaturesCreator.CreateRemoveProjectFromGroupService();
        addProjectToGroupService = projectFeaturesCreator.CreateAddProjectToGroupService();
        sortUpProjectService = projectFeaturesCreator.CreateSortUpProjectService();
        sortDownProjectService = projectFeaturesCreator.CreateSortDownProjectService();
        #endregion
        #region Queries
        getAllProjectService = projectFeaturesCreator.CreateGetAllProjectService();
        searchProjectService = projectFeaturesCreator.CreateSearchProjectService();
        getCurrentGitBranchService = gitFeaturesCreator.CreateGetCurrentGitBranchService();
        #endregion
        #endregion

        InitializeComponent();
        var presenter = new MainWindowPresenter(this);
        DataContext = MainWindowViewModel;
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
                projectPathsViewModel.Id == MainWindowViewModel?.SelectedProjectPath?.Id
            );

        lvProjectPaths.ScrollIntoView(this.lvProjectPaths.SelectedItem);
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        SearchProjectEvent.Invoke(this, EventArgs.Empty);
        FetchDevAppsEvent!.Invoke(this, EventArgs.Empty);
    }

    private void VsCodePathOpenDialogButton_Click(object sender, RoutedEventArgs e)
    {
        var openFolderDialog = new OpenFileDialog { Filter = "Executable Files | *.exe" };
        var result = openFolderDialog.ShowDialog() ?? false;

        if (!result)
        {
            return;
        }

        DevAppFilePath = openFolderDialog.FileName;
        AddNewDevAppEvent!.Invoke(this, EventArgs.Empty);
    }

    private void ProjectPathsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (lvProjectPaths.SelectedIndex == -1)
        {
            return;
        }

        SelectProjectEvent.Invoke(this, EventArgs.Empty);
    }

    private void btnOpenDialogProjectPath_Click(object sender, RoutedEventArgs e)
    {
        var openFolderDialog = new OpenFolderDialog();
        var result = openFolderDialog.ShowDialog() ?? false;

        if (!result)
        {
            return;
        }

        string filePath = openFolderDialog.FolderName;
        string name = openFolderDialog.SafeFolderName;
        MainWindowViewModel!.SelectedProjectPath = new() { Name = name, Path = filePath };
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
        DeleteDevAppEvent.Invoke(this, EventArgs.Empty);
    }

    private void btnNewProjectPath_Click(object sender, RoutedEventArgs e)
    {
        NewProjectEvent.Invoke(this, EventArgs.Empty);
    }

    private void btnDeleteProjectPath_Click(object sender, RoutedEventArgs e)
    {
        DeleteProjectEvent.Invoke(this, EventArgs.Empty);
    }

    private void mnuMoveUp_Click(object sender, RoutedEventArgs e)
    {
        SortUpProjectEvent.Invoke(this, EventArgs.Empty);
    }

    private void mnuMoveDown_Click(object sender, RoutedEventArgs e)
    {
        SortDownProjectEvent.Invoke(this, EventArgs.Empty);
    }

    private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
    {
        SearchProjectEvent.Invoke(this, EventArgs.Empty);
    }

    private void MnuOpenFolderWindow_Click(object sender, RoutedEventArgs e)
    {
        OpenProjectFolderWindowEvent.Invoke(this, EventArgs.Empty);
    }

    private void txtSearch_KeyUp(object sender, KeyEventArgs e)
    {
        if (e.Key != Key.Down)
        {
            return;
        }

        FocusOnListViewEvent.Invoke(this, EventArgs.Empty);
    }

    private void lvProjectPaths_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key != Key.Enter)
        {
            return;
        }

        OpenProjectDevAppEvent.Invoke(this, EventArgs.Empty);
    }

    private void mnuAddToGroup_Click(object sender, RoutedEventArgs e)
    {
        OpenAddToGroupModalWindowEvent.Invoke(this, EventArgs.Empty);
    }

    public void ShowNoSelectedProjectMessage()
    {
        MessageBox.Show("No Selected Project", "Select Project", MessageBoxButton.OK);
    }
}
