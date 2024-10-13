﻿using ApplicationCore.Common;
using ApplicationCore.Features.DevApps;
using ApplicationCore.Features.Groups;
using ApplicationCore.Features.Projects;
using ApplicationCore.Features.Sorting;
using Infrastructure.Models;
using Infrastructure.ViewModels;
using LibGit2Sharp;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Win32;
using System.Windows;
using System.Windows.Controls;
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
    private readonly ISortProject? projectSorting;
    private readonly IGroupQuery? groupQuery;
    private readonly INotificationMessageService notificationMessageService;
    private readonly IAddProjectService? addProjectService;
    private readonly IGetLastProjectService? getLastProjectService;
    private readonly IGetAllProjectService? getAllProjectService;
    private readonly IDeleteProjectService? deleteProjectService;
    private readonly IEditProjectService? editProjectService;
    private readonly ISearchProjectService searchProductService;
    private readonly IOpenProjectFolderWindowService openProjectFolderWindowService;
    private readonly IOpenProjectDevAppService openProjectDevAppService;
    private readonly MainWindowViewModel? mainWindowViewModel;
    private GroupModalWindow? groupModalWindow;
    private List<Group> groups = [];

    //public ObservableCollection<Infrastructure.Models.Project> Entries { get; private set; } = [];

    public MainWindow() => InitializeComponent();

    public MainWindow(
        IDevAppFeaturesCreator devAppFeaturesCreator,
        ISortProject? projectSorting,
        IGroupQuery groupQuery,
        IProjectFeaturesCreator projectFeaturesCreator,
        INotificationMessageService notificationMessageService

    )
    {
        this.addDevAppService = devAppFeaturesCreator.CreateAddDevAppService();
        this.editDevAppService = devAppFeaturesCreator.CreateEditDevAppService();
        this.deleteDevAppService = devAppFeaturesCreator.CreateDeleteDevAppService();
        this.getAllDevAppService = devAppFeaturesCreator.CreateGetAllDevAppService();
        this.getOneDevAppService = devAppFeaturesCreator.CreateGetOneDevAppService();
        this.projectSorting = projectSorting;
        this.groupQuery = groupQuery;
        this.notificationMessageService = notificationMessageService;
        this.addProjectService = projectFeaturesCreator.CreateAddProjectService();
        this.getLastProjectService = projectFeaturesCreator.CreateGetLastProjectService();
        this.getAllProjectService = projectFeaturesCreator.CreateGetAllProjectService();
        this.deleteProjectService = projectFeaturesCreator.CreateDeleteAddProjectService();
        this.editProjectService = projectFeaturesCreator.CreateEditAddProjectService();
        this.searchProductService = projectFeaturesCreator.CreateSearchProjectService();
        this.openProjectFolderWindowService = projectFeaturesCreator.CreateOpenProjectFolderWindowAppService();
        this.openProjectDevAppService = projectFeaturesCreator.CreateOpenProjectDevAppService();


        this.mainWindowViewModel = new MainWindowViewModel();
        DataContext = this.mainWindowViewModel;
        InitializeComponent();

    }


    private async void Window_Loaded(object sender, RoutedEventArgs e)
    {
        await this.FetchProjectPaths();
        await this.FetchIDEPaths();
    }

    private void NotificationMessageService_Notify(object? sender, NotificationMessageEventArgs e)
    {
        MessageBox.Show(e.Message);
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
        var resultSave = await this.addDevAppService!.Add(new() { Path = filePath });
        if (resultSave)
        {
            this.mainWindowViewModel!.IdePathsModels!.Clear();
            await this.FetchIDEPaths();
            this.mainWindowViewModel.SelectedIdePath = new();
            this.mainWindowViewModel.SelectedProjectPath = null;
            MessageBox.Show("IDE path saved!");
        }
    }

    private async Task FetchGroups()
    {
        this.groups = await this.groupQuery!.GetAll();
    }

    private async Task FetchProjectPaths()
    {
        var getAllProjectVm = await this.getAllProjectService!.Handle();
        this.mainWindowViewModel!.ProjectPathModels = [.. getAllProjectVm.Projects];

    }

    private async Task FetchIDEPaths()
    {
        var getAllDevAppVm = await this.getAllDevAppService!.Handle();
        this.mainWindowViewModel!.IdePathsModels = [.. getAllDevAppVm.DevApps];
    }

    private void ProjectPathsList_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {
        string branchName = "";
        if (lvProjectPaths.SelectedIndex == -1) return;

        var project = (ProjectViewModel)lvProjectPaths.SelectedItem;

        try
        {
            this.mainWindowViewModel!.SelectedIdePath = this.mainWindowViewModel!.IdePathsModels!.Single(x => x.Id == project.IDEPathId);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }

        try
        {
            using var repo = new Repository(project.Path);

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

        this.mainWindowViewModel!.SelectedProjectPath = project;

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
        if (this.mainWindowViewModel!.SelectedProjectPath!.Id == 0)
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

            this.mainWindowViewModel.SelectedProjectPath.Id = (await this.getLastProjectService!.GetLast()).Id;
            this.mainWindowViewModel!.ProjectPathModels!.Clear();
            await this.FetchProjectPaths();
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
                this.mainWindowViewModel!.SelectedProjectPath!.IDEPathId,
                this.mainWindowViewModel!.SelectedProjectPath!.Filename
            )
        );
        this.mainWindowViewModel!.ProjectPathModels!.Clear();
        await this.FetchProjectPaths();
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

        this.openProjectDevAppService.Handle
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
        if (this.mainWindowViewModel!.SelectedIdePath!.Id == 0) return;

        try
        {
            var result = await this.deleteDevAppService!.Delete(new() { Id = this.mainWindowViewModel!.SelectedIdePath!.Id });

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

        var result = await this.deleteProjectService.Delete(this.mainWindowViewModel!.SelectedProjectPath!.Id);

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

    private async void TxtSearch_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
    {
        await Search();
    }

    private async Task Search()
    {

        var searchViewModel = await this.searchProductService.Handle(new() { Search = this.mainWindowViewModel!.Search });
        this.mainWindowViewModel!.ProjectPathModels = [.. searchViewModel.Projects];

    }
    private void MnuOpenFolderWindow_Click(object sender, RoutedEventArgs e)
    {
        this.openProjectFolderWindowService.Handle
        (
            new() { Path = this.mainWindowViewModel!.SelectedProjectPath!.Path }
        );
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
        await this.Search();
        SelectEditedItem();
        groupModalWindow!.Close();
    }
}

