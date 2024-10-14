using ApplicationCore.Features.DevApps;
using ApplicationCore.Features.Git;
using ApplicationCore.Features.Groups;
using ApplicationCore.Features.Projects;
using Infrastructure.Models;
using Infrastructure.ViewModels;
using Microsoft.Win32;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using UI.Windows.Group;

namespace UI;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private readonly IAddDevAppService? addDevAppService;
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
    private readonly ISortUpProjectService sortUpProjectService;
    private readonly ISortDownProjectService sortDownProjectService;
    private readonly MainWindowViewModel? mainWindowViewModel;
    private GroupModalWindow? groupModalWindow;
    private readonly List<Group> groups = [];

    public MainWindow() => InitializeComponent();

    public MainWindow(
        IDevAppFeaturesCreator devAppFeaturesCreator,
        IProjectFeaturesCreator projectFeaturesCreator,
        IGitFeaturesCreator gitFeaturesCreator,
        IGroupFeaturesCreator groupFeaturesCreator
    )
    {
        this.addDevAppService = devAppFeaturesCreator.CreateAddDevAppService();
        this.editDevAppService = devAppFeaturesCreator.CreateEditDevAppService();
        this.deleteDevAppService = devAppFeaturesCreator.CreateDeleteDevAppService();
        this.getAllDevAppService = devAppFeaturesCreator.CreateGetAllDevAppService();
        this.getOneDevAppService = devAppFeaturesCreator.CreateGetOneDevAppService();
        this.projectFeaturesCreator = projectFeaturesCreator;
        this.getAllGroupService = groupFeaturesCreator.CreateGetAllGroupService();
        this.addProjectService = projectFeaturesCreator.CreateAddProjectService();
        this.getAllProjectService = projectFeaturesCreator.CreateGetAllProjectService();
        this.deleteProjectService = projectFeaturesCreator.CreateDeleteAddProjectService();
        this.editProjectService = projectFeaturesCreator.CreateEditAddProjectService();
        this.searchProductService = projectFeaturesCreator.CreateSearchProjectService();
        this.openProjectFolderWindowService = projectFeaturesCreator.CreateOpenProjectFolderWindowAppService();
        this.openProjectDevAppService = projectFeaturesCreator.CreateOpenProjectDevAppService();
        this.getCurrentGitBranchService = gitFeaturesCreator.CreateGetCurrentGitBranchService();
        this.removeProjectFromGroupService = projectFeaturesCreator.CreateRemoveProjectFromGroupService();
        this.addProjectToGroupService = projectFeaturesCreator.CreateAddProjectToGroupService();
        this.sortUpProjectService = projectFeaturesCreator.CreateSortUpProjectService();
        this.sortDownProjectService = projectFeaturesCreator.CreateSortDownProjectService();
        this.mainWindowViewModel = new MainWindowViewModel();
        DataContext = this.mainWindowViewModel;
        InitializeComponent();
    }


    private async void Window_Loaded(object sender, RoutedEventArgs e)
    {
        this.removeProjectFromGroupService!.Notify += RemoveProjectFromGroupNotificationService_Notify; ;
        this.addProjectToGroupService!.Notify += AddProjectToGroupService_Notify;
        await this.FetchProjects();
        await this.FetchDevApps();
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

    private async void VsCodePathOpenDialogButton_Click(object sender, RoutedEventArgs e)
    {
        await OpenDevAppDialog();
    }

    private async Task OpenDevAppDialog()
    {
        var openFolderDialog = new OpenFileDialog
        {
            Filter = "Executable Files | *.exe"
        };
        var result = openFolderDialog.ShowDialog() ?? false;

        if (!result)
        {
            return;
        }

        string filePath = openFolderDialog.FileName;
        var resultSave = await this.addDevAppService!.Add(new() { Path = filePath });
        if (resultSave)
        {
            await this.FetchDevApps();
            this.mainWindowViewModel!.SelectedIdePath = new();
        }
    }

    private async Task FetchProjects()
    {
        var getAllProjectVm = await this.getAllProjectService!.Handle();
        this.mainWindowViewModel!.ProjectPathModels = [.. getAllProjectVm.Projects];
    }

    private async Task FetchDevApps()
    {
        var getAllDevAppVm = await this.getAllDevAppService!.Handle();
        this.mainWindowViewModel!.IdePathsModels = [.. getAllDevAppVm.DevApps];
    }

    private void ProjectPathsList_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {
        SelectProject();
    }

    private void SelectProject()
    {
        if (lvProjectPaths.SelectedIndex == -1) { return; }
        var project = (ProjectViewModel)lvProjectPaths.SelectedItem;
        var currentGitBranch = getCurrentGitBranchService!.Handle(new() { DirectoryPath = project.Path });
        project.CurrentGitBranch = currentGitBranch;
        this.mainWindowViewModel!.SelectedProjectPath = project;
        // Selected Dev App : Must be same reference, data must be on view model list to set the value
        this.mainWindowViewModel!.SelectedIdePath = this.mainWindowViewModel!.IdePathsModels!.First(x => x.Id == project.IDEPathId);
    }

    private void btnOpenDialogProjectPath_Click(object sender, RoutedEventArgs e)
    {
        OpenProjectDialog();
    }

    private void OpenProjectDialog()
    {
        var openFolderDialog = new OpenFolderDialog();
        var result = openFolderDialog.ShowDialog() ?? false;

        if (result)
        {
            string filePath = openFolderDialog.FolderName;
            string name = openFolderDialog.SafeFolderName;

            this.mainWindowViewModel!.SelectedProjectPath = new()
            {
                Path = filePath!,
                Name = name!
            };
        }
    }

    private async void btnSaveProjectPath_Click(object sender, RoutedEventArgs e)
    {
        await SaveProject();
    }

    private async Task SaveProject()
    {
        if (this.mainWindowViewModel!.SelectedProjectPath?.Id == 0)
        {
            await this.addProjectService!.AddAsync
            (
                new
                (
                    this.mainWindowViewModel!.SelectedProjectPath!.Name,
                    this.mainWindowViewModel!.SelectedProjectPath!.Path,
                    this.mainWindowViewModel!.SelectedIdePath!.Id,
                    this.mainWindowViewModel!.SelectedProjectPath!.Filename
                )
            );

            await this.Search();
            SelectNewlyAddedItem();
            return;
        }

        await this.editProjectService!.Edit
        (
            new
            (
                this.mainWindowViewModel!.SelectedProjectPath!.Id,
                this.mainWindowViewModel!.SelectedProjectPath!.Name,
                this.mainWindowViewModel!.SelectedProjectPath!.Path,
                this.mainWindowViewModel!.SelectedIdePath!.Id,
                this.mainWindowViewModel!.SelectedProjectPath!.Filename
            )
        );

        await this.Search();
        SelectEditedItem();
    }

    private void SelectEditedItem()
    {
        lvProjectPaths.SelectedItem = lvProjectPaths.Items.SourceCollection
                 .Cast<ProjectViewModel>()
                 .FirstOrDefault(projectPathsViewModel => projectPathsViewModel.Id == this.mainWindowViewModel?.SelectedProjectPath?.Id);

        lvProjectPaths.ScrollIntoView(this.lvProjectPaths.SelectedItem);
    }

    private void SelectNewlyAddedItem()
    {
        lvProjectPaths.SelectedItem = lvProjectPaths.Items[^1];
        lvProjectPaths.ScrollIntoView(this.lvProjectPaths.SelectedItem);
    }

    private void ProjectPathsList_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        OpenProject();
    }

    private void OpenProject()
    {
        var project = this.mainWindowViewModel!.SelectedProjectPath;

        if (project is null)
        {
            return;
        }

        this.openProjectDevAppService!.Handle
        (
            new()
            {
                DevAppPath = project.DevAppPath,
                DirectoryPath = project.Path,
                FullFilePath = project.FullPath,
                HasFileName = project.HasFileName,
            }
        );
    }

    private async void btnDeleteIdePath_Click(object sender, RoutedEventArgs e)
    {
        await DeleteDevApp();
    }

    private async Task DeleteDevApp()
    {
        if (this.mainWindowViewModel!.SelectedIdePath!.Id == 0) return;

        var result = await this.deleteDevAppService!.Delete(new() { Id = this.mainWindowViewModel!.SelectedIdePath!.Id });

        if (result)
        {
            await this.FetchDevApps();
            this.mainWindowViewModel!.SelectedIdePath = new();
        }
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

    private async Task DeleteProject()
    {
        if (this.mainWindowViewModel!.SelectedProjectPath!.Id == 0) return;

        var result = await this.deleteProjectService!.Delete(this.mainWindowViewModel!.SelectedProjectPath!.Id);

        if (result)
        {
            await Search();
            this.mainWindowViewModel!.SelectedProjectPath = new();
            this.mainWindowViewModel!.SelectedIdePath = new();
        }
    }

    private async void mnuMoveUp_Click(object sender, RoutedEventArgs e)
    {
        await SortUpProject();
    }

    private async Task SortUpProject()
    {
        var result = await this.sortUpProjectService!.Handle(new() { SortId = this.mainWindowViewModel!.SelectedProjectPath!.SortId });
        if (!result)
        {
            return;
        }

        await this.FetchProjects();
    }

    private async void mnuMoveDown_Click(object sender, RoutedEventArgs e)
    {
        await SortDownProject();
    }

    private async Task SortDownProject()
    {
        var result = await this.sortDownProjectService.Handle(new() { SortId = this.mainWindowViewModel!.SelectedProjectPath!.SortId });
        if (!result)
        {
            return;
        }
        await this.FetchProjects();
    }

    private async void TxtSearch_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
    {
        await Search();
    }

    private async Task Search()
    {
        var searchViewModel = await this.searchProductService!.Handle(new() { Search = this.mainWindowViewModel!.Search });
        this.mainWindowViewModel.EnableAddNewProject = searchViewModel.EnableAddNewProject;
        this.mainWindowViewModel!.ProjectPathModels = [.. searchViewModel.Projects];

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

    private void FocusOnListViewWhenArrowDown(KeyEventArgs e)
    {
        if (e.Key != Key.Down)
        {
            return;
        }

        if (lvProjectPaths.Items.Count == 0)
        {
            return;
        }

        lvProjectPaths.Focus();
        var item = lvProjectPaths.Items[0];
        lvProjectPaths.SelectedItem = item;
        ListViewItem? firstListViewItem = lvProjectPaths.ItemContainerGenerator.ContainerFromIndex(0) as ListViewItem;
        firstListViewItem?.Focus();
    }

    private void lvProjectPaths_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
    {
        OpenProjectWhenEnter(e);
    }

    private void OpenProjectWhenEnter(KeyEventArgs e)
    {
        if (e.Key != Key.Enter)
        {
            return;
        }

        OpenProject();
    }

    private void mnuAddToGroup_Click(object sender, RoutedEventArgs e)
    {
        OpenAddToGroupDialog();
    }

    private void OpenAddToGroupDialog()
    {
        this.groupModalWindow = new GroupModalWindow
        (
            getAllGroupService!,
            addProjectToGroupService!,
            removeProjectFromGroupService!
        );

        this.groupModalWindow!.ProjectPath = this.mainWindowViewModel!.SelectedProjectPath;
        groupModalWindow?.ShowDialog();
    }
}