using System.Windows;
using ApplicationCore.Features.DevApps;
using ApplicationCore.Features.Git;
using ApplicationCore.Features.Groups;
using ApplicationCore.Features.Projects;
using Infrastructure.ViewModels;
using UI.Windows.Group;

namespace UI.MainWindows;

public interface IMainWindowView
{
    event EventHandler AddNewDevAppEvent;
    event EventHandler FetchDevAppsEvent;
    event EventHandler DeleteDevAppEvent;
    event EventHandler NewProjectEvent;

    event EventHandler DeleteProjectEvent;
    event EventHandler SearchProjectEvent;
    event EventHandler SortUpProjectEvent;
    event EventHandler SelectProjectEvent;
    event EventHandler SortDownProjectEvent;
    event EventHandler OpenAddToGroupModalWindowEvent;
    event EventHandler SaveProjectEvent;
    event EventHandler OpenProjectDevAppEvent;
    event EventHandler OpenProjectFolderWindowEvent;
    event EventHandler FocusOnListViewEvent;

    string DevAppFilePath { get; set; }
    IAddDevAppService? AddDevAppService { get; }
    IGetAllDevAppService? GetAllDevAppService { get; }
    IDeleteDevAppService? DeleteDevAppService { get; }
    IDeleteProjectService? DeleteProjectService { get; }
    ISearchProjectService? SearchProjectService { get; }
    ISortUpProjectService? SortUpProjectService { get; }
    IGetCurrentGitBranchService? GetCurrentGitBranchService { get; }
    ISortDownProjectService? SortDownProjectService { get; }
    IGetAllGroupService? GetAllGroupService { get; }
    IAddProjectToGroupService? AddProjectToGroupService { get; }
    IRemoveProjectFromGroupService? RemoveProjectFromGroupService { get; }
    IAddProjectService? AddProjectService { get; }
    IEditProjectService? EditProjectService { get; }
    IOpenProjectDevAppService? OpenProjectDevAppService { get; }
    IOpenProjectFolderWindowService? OpenProjectFolderWindowService { get; }

    MainWindowViewModel MainWindowViewModel { get; set; }

    void FocusOnListViewWhenArrowDown();
    void SelectNewlyAddedItem();
    void SelectEditedItem();
    ProjectViewModel SelectedProject { get; }
    void ShowNoSelectedProjectMessage();
}

public class MainWindowPresenter
{
    private readonly IMainWindowView view;
    private GroupModalWindow? groupModalWindow;

    public MainWindowPresenter(IMainWindowView view)
    {
        view.AddNewDevAppEvent += Presenter_AddNewDevAppEvent;
        view.FetchDevAppsEvent += Presenter_FetchDevAppsEvent;
        view.DeleteDevAppEvent += Presenter_DeleteDevAppEvent;
        view.NewProjectEvent += Presenter_NewProjectEvent;
        view.DeleteProjectEvent += Presenter_DeleteProjectEvent;
        view.SearchProjectEvent += Presenter_SearchProjectEvent;
        view.SortUpProjectEvent += Presenter_SortUpProjectEvent;
        view.SelectProjectEvent += Presenter_SelectProjectEvent;
        view.SortDownProjectEvent += Presenter_SortDownProjectEvent;
        view.OpenAddToGroupModalWindowEvent += Presenter_OpenAddToGroupModalWindowEvent;
        view.AddProjectToGroupService!.Notify += MainWindowPresenter_Notify;
        view.RemoveProjectFromGroupService!.Notify += RemoveProjectFromGroup_Notify;
        view.SaveProjectEvent += Presenter_SaveProjectEvent;
        view.OpenProjectDevAppEvent += Presenter_OpenProjectDevAppEvent;

        view.OpenProjectFolderWindowEvent += Presenter_OpenFolderWindowEvent;
        view.FocusOnListViewEvent += Presenter_FocusOnListViewEvent;
        this.view = view;

        if (this.view.MainWindowViewModel is null)
        {
            this.view.MainWindowViewModel = new();
        }
        else
        {
            MessageBox.Show("Instance of MainWindowViewModel was created earlier!");
        }
    }

    private void Presenter_FocusOnListViewEvent(object? sender, EventArgs e)
    {
        view.FocusOnListViewWhenArrowDown();
    }

    private void Presenter_OpenFolderWindowEvent(object? sender, EventArgs e)
    {
        this.view.OpenProjectFolderWindowService!.Handle(
            new() { Path = this.view.MainWindowViewModel!.SelectedProjectPath!.Path }
        );
    }

    private void Presenter_OpenProjectDevAppEvent(object? sender, EventArgs e)
    {
        this.OpenProjectDevApp();
    }

    private async void Presenter_SaveProjectEvent(object? sender, EventArgs e)
    {
        if (this.view.MainWindowViewModel!.SelectedProjectPath?.Id == 0)
        {
            var addResult = await this.view.AddProjectService.AddAsync(
                new(
                    this.view.MainWindowViewModel!.SelectedProjectPath!.Name,
                    this.view.MainWindowViewModel!.SelectedProjectPath!.Path,
                    this.view.MainWindowViewModel!.SelectedIdePath!.Id,
                    this.view.MainWindowViewModel!.SelectedProjectPath!.Filename
                )
            );

            if (addResult)
            {
                await Search();
                view.SelectNewlyAddedItem();
            }
            return;
        }

        var editResult = await this.view.EditProjectService!.Edit(
            new(
                this.view.MainWindowViewModel!.SelectedProjectPath!.Id,
                this.view.MainWindowViewModel!.SelectedProjectPath!.Name,
                this.view.MainWindowViewModel!.SelectedProjectPath!.Path,
                this.view.MainWindowViewModel!.SelectedIdePath!.Id,
                this.view.MainWindowViewModel!.SelectedProjectPath!.Filename
            )
        );

        if (editResult)
        {
            await Search();
            view.SelectEditedItem();
        }
    }

    private async void RemoveProjectFromGroup_Notify(
        object? sender,
        RemoveProjectFromGroupEventArgs e
    )
    {
        await Search();
        view.SelectEditedItem();
        groupModalWindow!.Close();
    }

    private async void MainWindowPresenter_Notify(object? sender, EventArgs e)
    {
        await this.Search();
        view.SelectEditedItem();
        groupModalWindow!.Close();
    }

    private void Presenter_OpenAddToGroupModalWindowEvent(object? sender, EventArgs e)
    {
        this.groupModalWindow = new GroupModalWindow(
            view.GetAllGroupService,
            view.AddProjectToGroupService,
            view.RemoveProjectFromGroupService
        );

        this.groupModalWindow!.ProjectPath = this.view.MainWindowViewModel.SelectedProjectPath;

        groupModalWindow?.ShowDialog();
    }

    private async void Presenter_SortDownProjectEvent(object? sender, EventArgs e)
    {
        var result = await this.view.SortDownProjectService!.Handle(
            new() { SortId = this.view.MainWindowViewModel!.SelectedProjectPath!.SortId }
        );

        if (!result)
        {
            view.ShowNoSelectedProjectMessage();
            return;
        }

        await this.Search();
    }

    private void Presenter_SelectProjectEvent(object? sender, EventArgs e)
    {
        var project = view.SelectedProject;

        var currentGitBranch = view.GetCurrentGitBranchService!.Handle(
            new() { DirectoryPath = project.Path }
        );
        project.CurrentGitBranch = currentGitBranch;
        this.view.MainWindowViewModel!.SelectedProjectPath = project.Copy();

        // Selected Dev App : Must be same reference, data must be on view model list to set the value
        this.view.MainWindowViewModel!.SelectedIdePath =
            this.view.MainWindowViewModel!.IdePathsModels!.First(x => x.Id == project.IDEPathId);
    }

    private async void Presenter_SortUpProjectEvent(object? sender, EventArgs e)
    {
        var result = await this.view.SortUpProjectService!.Handle(
            new() { SortId = this.view.MainWindowViewModel!.SelectedProjectPath!.SortId }
        );

        if (!result)
        {
            view.ShowNoSelectedProjectMessage();
            return;
        }

        await this.Search();
    }

    private async void Presenter_SearchProjectEvent(object? sender, EventArgs e)
    {
        await this.Search();
    }

    private async void Presenter_DeleteProjectEvent(object? sender, EventArgs e)
    {
        if (this.view.MainWindowViewModel!.SelectedProjectPath!.Id == 0)
        {
            view.ShowNoSelectedProjectMessage();
            return;
        }

        var result = await this.view.DeleteProjectService!.Delete(
            this.view.MainWindowViewModel!.SelectedProjectPath!.Id
        );

        if (result)
        {
            await Search();
            this.view.MainWindowViewModel!.SelectedProjectPath = new();
            this.view.MainWindowViewModel!.SelectedIdePath = new();
        }
    }

    private async Task Search()
    {
        var searchViewModel = await this.view.SearchProjectService!.Handle(
            new() { Search = this.view.MainWindowViewModel!.Search }
        );
        this.view.MainWindowViewModel.EnableAddNewProject = searchViewModel.EnableAddNewProject;
        this.view.MainWindowViewModel!.ProjectPathModels = [.. searchViewModel.Projects];
    }

    private void Presenter_NewProjectEvent(object? sender, EventArgs e)
    {
        this.view.MainWindowViewModel!.SelectedProjectPath = new();
        this.view.MainWindowViewModel!.SelectedIdePath = new();
    }

    private async void Presenter_DeleteDevAppEvent(object? sender, EventArgs e)
    {
        if (this.view.MainWindowViewModel!.SelectedIdePath!.Id == 0)
            return;

        var result = await this.view.DeleteDevAppService!.Delete(
            new() { Id = this.view.MainWindowViewModel!.SelectedIdePath!.Id }
        );

        if (result)
        {
            var getAllDevAppVm = await view.GetAllDevAppService!.Handle();
            view.MainWindowViewModel!.IdePathsModels = [.. getAllDevAppVm.DevApps];
        }
    }

    private async void Presenter_FetchDevAppsEvent(object? sender, EventArgs e)
    {
        var getAllDevAppVm = await view.GetAllDevAppService.Handle();
        view.MainWindowViewModel!.IdePathsModels = [.. getAllDevAppVm.DevApps];
    }

    private async void Presenter_AddNewDevAppEvent(object? sender, EventArgs e)
    {
        var resultSave = await this.view.AddDevAppService!.Add(
            new() { Path = this.view.DevAppFilePath }
        );

        if (resultSave)
        {
            var getAllDevAppVm = await view.GetAllDevAppService.Handle();
            view.MainWindowViewModel!.IdePathsModels = [.. getAllDevAppVm.DevApps];

            view.MainWindowViewModel!.SelectedIdePath = new();
        }
    }

    private void OpenProjectDevApp()
    {
        var project = this.view.MainWindowViewModel!.SelectedProjectPath;

        if (project is null)
        {
            view.ShowNoSelectedProjectMessage();
            return;
        }

        view.OpenProjectDevAppService!.Handle(
            new()
            {
                DevAppPath = project.DevAppPath,
                DirectoryPath = project.Path,
                FullFilePath = project.FullPath,
                HasFileName = project.HasFileName,
            }
        );
    }
}
