using ApplicationCore.Features.Basic.Group;
using ApplicationCore.Features.Basic.Project;
using ApplicationCore.Features.Sorting;
using Infrastructure.Database;
using Infrastructure.IDEPath;
using Infrastructure.ViewModels;
using LibGit2Sharp;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Win32;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using UI.Windows.Group;

namespace UI;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private readonly IGetIDEPath? getVsCodePath;
    private readonly ISaveIDEPath? saveVsCodePath;
    private readonly IGetIDEPaths? getIDEPaths;
    private readonly IDeleteIdePath? deleteIdePath;
    private readonly ISortProject? projectSorting;
    private readonly IGroupDataService? groupQuery;
    private readonly IProjectDataService? projectDataService;
    private readonly MainWindowViewModel? mainWindowViewModel;
    private ImmutableList<ProjectPathsViewModel> projectPaths = [];
    private GroupModalWindow? groupModalWindow;
    private List<GroupDetail> groups = [];

    public ObservableCollection<Infrastructure.Models.Project> Entries { get; private set; } = [];

    public MainWindow() => InitializeComponent();

    public MainWindow(
        IInitializedDatabaseMigration initializedDatabaseMigration,
        IGetIDEPath getVsCodePath,
        ISaveIDEPath saveIdePath,
        IGetIDEPaths getIDEPaths,
        IDeleteIdePath deleteIdePath,
        ISortProject? projectSorting,
        IGroupDataService groupQuery,
        IProjectDataService projectPersistence
    )
    {
        initializedDatabaseMigration.Execute();
        this.getVsCodePath = getVsCodePath;
        this.saveVsCodePath = saveIdePath;
        this.getIDEPaths = getIDEPaths;
        this.deleteIdePath = deleteIdePath;
        this.projectSorting = projectSorting;
        this.groupQuery = groupQuery;
        this.projectDataService = projectPersistence;

        this.mainWindowViewModel = new MainWindowViewModel();
        DataContext = this.mainWindowViewModel;
        InitializeComponent();

    }

    private async void Window_Loaded(object sender, RoutedEventArgs e)
    {
        await this.FetchProjectPaths();
        await this.FetchIDEPaths();
    }

    private async void VsCodePathOpenDialogButton_Click(object sender, RoutedEventArgs e)
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
        var resultSave = await this.saveVsCodePath!.ExecuteAsync(filePath);
        if (resultSave)
        {
            this.mainWindowViewModel!.IdePathsModels!.Clear();
            await this.FetchIDEPaths();
            this.mainWindowViewModel.SelectedIdePath = new();
            this.mainWindowViewModel.SelectedProjectPath = new();
            MessageBox.Show("IDE path saved!");
        }
    }

    private async Task FetchGroups()
    {
        this.groups = await this.groupQuery!.GetAll();
    }

    private async Task FetchProjectPaths()
    {
        if (this.groups.Count is 0)
        {
            await this.FetchGroups();
        }

        var projectPaths = await this.projectDataService!.GetAll();


        if (this.getVsCodePath == null) return;

        this.projectPaths = [.. projectPaths];
        this.mainWindowViewModel!.ProjectPathModels = [.. projectPaths];

    }

    private async Task FetchIDEPaths()
    {

        var idePaths = await this.getIDEPaths!.ExecuteAsync();

        if (this.getVsCodePath == null) return;

        foreach (var item in idePaths.Select((value, index) => (value, index)))
        {
            var (value, index) = item;
            index++;

            this.mainWindowViewModel!.IdePathsModels?.Add(new() { Id = value.Id, Path = value.Path });
        }
    }

    private void ProjectPathsList_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {
        string branchName = "";
        if (lvProjectPaths.SelectedIndex == -1) return;

        var projectPath = (ProjectPathsViewModel)lvProjectPaths.SelectedItem;

        try
        {
            this.mainWindowViewModel!.SelectedIdePath = this.mainWindowViewModel!.IdePathsModels!.Single(x => x.Id == projectPath.IDEPathId);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }

        try
        {
            using var repo = new Repository(projectPath.Path);

            if (repo == null) { return; }

            var branch = repo.Branches.FirstOrDefault(branch => branch.IsCurrentRepositoryHead);

            branchName = branch?.FriendlyName ?? "";
        }
        catch (RepositoryNotFoundException)
        {
            branchName = "No Git Repository for this project!";
        }
        catch (Exception ex)
        {
            branchName = $"Other errors thrown: {ex.Message}";
        }

        this.mainWindowViewModel!.SelectedProjectPath = ProjectPathViewModel.Transform(projectPath, branchName);

    }

    private void btnOpenDialogProjectPath_Click(object sender, RoutedEventArgs e)
    {
        var openFolderDialog = new OpenFolderDialog();
        var result = openFolderDialog.ShowDialog() ?? false;

        if (result)
        {
            string filePath = openFolderDialog.FolderName;
            string name = openFolderDialog.SafeFolderName;

            this.mainWindowViewModel!.SelectedProjectPath = new() { Id = this.mainWindowViewModel!.SelectedProjectPath!.Id, Path = filePath!, Name = name! };

        }
    }

    private async void btnSaveProjectPath_Click(object sender, RoutedEventArgs e)
    {
        bool result;

        this.mainWindowViewModel!.SelectedProjectPath!.IDEPathId = this.mainWindowViewModel!.SelectedIdePath!.Id;

        if (this.mainWindowViewModel!.SelectedProjectPath!.Id == 0)
        {
            result = await this.projectDataService!.Add(this.mainWindowViewModel!.SelectedProjectPath!);
            this.mainWindowViewModel.SelectedProjectPath.Id = (await this.projectDataService!.GetLast()).Id;
            this.mainWindowViewModel!.ProjectPathModels!.Clear();
            await this.FetchProjectPaths();
            this.Search();
            SelectNewlyAddedItem();
        }
        else
        {
            result = await this.projectDataService!.Edit(this.mainWindowViewModel.SelectedProjectPath);
            this.mainWindowViewModel!.ProjectPathModels!.Clear();
            await this.FetchProjectPaths();
            this.Search();
            SelectEditedItem();
        }

        if (!result)
        {
            return;
        }

        MessageBox.Show("Project path saved!");
    }

    private void SelectEditedItem()
    {
        lvProjectPaths.SelectedItem = lvProjectPaths.Items.SourceCollection
                 .Cast<ProjectPathsViewModel>()
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
        this.OpenProject();
    }

    private void OpenProject()
    {
        try
        {
            if (!Directory.Exists(this.mainWindowViewModel!.SelectedProjectPath!.Path))
            {
                MessageBox.Show("Directory not found!", "Launch IDE Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (this.mainWindowViewModel.SelectedProjectPath.Filename is not "")
            {
                this.OpenIDEWithFileName();
                return;
            }

            this.OpenIDE
            (
                new()
                {
                    FileName = this.mainWindowViewModel!.SelectedIdePath!.Path,
                    Arguments = $"{this.mainWindowViewModel.SelectedProjectPath.Path}",
                    UseShellExecute = true,
                }
            );
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Launch IDE Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void OpenIDEWithFileName()
    {
        var file = $"{this.mainWindowViewModel?.SelectedProjectPath?.Path}\\{this.mainWindowViewModel?.SelectedProjectPath?.Filename}";

        if (File.Exists(file) is false)
        {
            MessageBox.Show("File not found!", "Open Project Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        this.OpenIDE
        (
            new()
            {
                FileName = this.mainWindowViewModel!.SelectedIdePath!.Path,
                Arguments = $"{file}",
                UseShellExecute = true,
            }
        );
    }

    private void OpenIDE(ProcessStartInfo processInfo)
    {
        using Process process = new();
        process.StartInfo = processInfo;
        process.Start();
    }


    private async void btnDeleteIdePath_Click(object sender, RoutedEventArgs e)
    {
        if (this.mainWindowViewModel!.SelectedIdePath!.Id == 0) return;

        try
        {
            var result = await this.deleteIdePath!.ExecuteAsync(this.mainWindowViewModel!.SelectedIdePath!.Id);

            if (result)
            {
                this.mainWindowViewModel!.IdePathsModels!.Clear();
                await this.FetchIDEPaths();
                this.mainWindowViewModel!.SelectedIdePath = new();
            }
        }
        catch (Exception ex)
        {

            MessageBox.Show(ex.Message);
        }
    }

    private void btnNewProjectPath_Click(object sender, RoutedEventArgs e)
    {
        this.mainWindowViewModel!.SelectedProjectPath = new();
        this.mainWindowViewModel!.SelectedIdePath = new();

    }

    private async void btnDeleteProjectPath_Click(object sender, RoutedEventArgs e)
    {
        if (this.mainWindowViewModel!.SelectedProjectPath!.Id == 0) return;

        var result = await this.projectDataService!.Delete(this.mainWindowViewModel!.SelectedProjectPath!.Id);

        if (result)
        {
            this.mainWindowViewModel!.ProjectPathModels!.Clear();
            await this.FetchProjectPaths();
            this.mainWindowViewModel!.SelectedProjectPath = new();
            this.mainWindowViewModel!.SelectedIdePath = new();
        }
    }

    private async void mnuMoveUp_Click(object sender, RoutedEventArgs e)
    {
        await this.projectSorting!.SortUp(this.mainWindowViewModel!.SelectedProjectPath!.SortId);
        this.mainWindowViewModel.ProjectPathModels?.Clear();
        await this.FetchProjectPaths();
    }

    private async void mnuMoveDown_Click(object sender, RoutedEventArgs e)
    {
        await this.projectSorting!.SortDown(this.mainWindowViewModel!.SelectedProjectPath!.SortId);
        this.mainWindowViewModel.ProjectPathModels?.Clear();
        await this.FetchProjectPaths();
    }

    private void TxtSearch_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
    {
        Search();
    }

    private void Search()
    {
        var filteredPaths = this.projectPaths.Where(projectPath => projectPath.Name.ToLower().Contains(this.mainWindowViewModel!.Search.ToLower()));
        this.mainWindowViewModel!.ProjectPathModels!.Clear();

        if (txtSearch.Text is not "")
        {
            btnNewProjectPath.IsEnabled = false;
            filteredPaths = filteredPaths.Select
            (
                projectPath => new ProjectPathsViewModel
                {
                    EnableMoveDown = false,
                    EnableMoveUp = false,
                    Filename = projectPath.Filename,
                    Id = projectPath.Id,
                    IDEPathId = projectPath.IDEPathId,
                    Index = projectPath.Index,
                    Name = projectPath.Name,
                    Path = projectPath.Path,
                    SortId = projectPath.SortId,
                    GroupId = projectPath.GroupId,
                    GroupName = projectPath.GroupName,
                }
            );
        }
        else
        {
            btnNewProjectPath.IsEnabled = true;
            filteredPaths = filteredPaths.Select
            (
                projectPath => new ProjectPathsViewModel
                {
                    EnableMoveUp = projectPath.Index != 1,
                    EnableMoveDown = projectPath.Index != projectPaths.Count,
                    Filename = projectPath.Filename,
                    Id = projectPath.Id,
                    IDEPathId = projectPath.IDEPathId,
                    Index = projectPath.Index,
                    Name = projectPath.Name,
                    Path = projectPath.Path,
                    SortId = projectPath.SortId,
                    GroupId = projectPath.GroupId,
                    GroupName = projectPath.GroupName,
                }
            );
        }

        foreach (var item in filteredPaths!)
        {
            this.mainWindowViewModel!.ProjectPathModels.Add(item);
        }
    }
    private void MnuOpenFolderWindow_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            if (!Directory.Exists(this.mainWindowViewModel!.SelectedProjectPath!.Path))
            {
                MessageBox.Show("Directory not found!", "Launch IDE Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            ProcessStartInfo startInfo = new()
            {
                FileName = "explorer.exe",
                Arguments = this.mainWindowViewModel.SelectedProjectPath.Path,
                UseShellExecute = true,

            };
            Process.Start(startInfo);

        }
        catch (Exception ex)
        {

            MessageBox.Show(ex.Message, "Launch IDE Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void txtSearch_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
    {
        if (e.Key != System.Windows.Input.Key.Down)
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
        if (e.Key != System.Windows.Input.Key.Enter)
        {
            return;
        }

        OpenProject();
    }

    private void mnuAddToGroup_Click(object sender, RoutedEventArgs e)
    {
        this.groupModalWindow = App.GetCurrentServiceProvider().GetService<GroupModalWindow>();
        this.groupModalWindow!.ProjectPath = this.mainWindowViewModel!.SelectedProjectPath;
        groupModalWindow!.OnSave += GroupModalWindow_OnSave;
        groupModalWindow?.ShowDialog();
    }

    private async void GroupModalWindow_OnSave(object? sender, EventArgs e)
    {
        this.mainWindowViewModel!.ProjectPathModels!.Clear();
        await this.FetchProjectPaths();
        this.Search();
        SelectEditedItem();
        groupModalWindow!.Close();
    }
}

