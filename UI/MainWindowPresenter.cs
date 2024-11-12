using System.Windows;
using System.Windows.Controls;
using ApplicationCore.Features.DevApps;
using ApplicationCore.Features.Git;
using ApplicationCore.Features.Groups;
using ApplicationCore.Features.Projects;
using Infrastructure.ViewModels;
using Microsoft.Win32;
using UI.Windows.Group;

namespace UI.MainWindowx.Presenter
{
    public interface IMainWindowPresenter
    {
        event EventHandler OpenDevApp;
        event EventHandler FetchDevAppsEvent;
        event EventHandler DeleteDevAppEvent;
        event EventHandler NewProjectEvent;
        event EventHandler OpenProjectDialog;
        event EventHandler DeleteProjectEvent;
        event EventHandler SearchProjectEvent;
        event EventHandler SortUpProjectEvent;
        event EventHandler SelectProjectEvent;
        event EventHandler SortDownProjectEvent;
        event EventHandler OpenAddToGroupModalWindowEvent;
        event EventHandler SaveProjectEvent;

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

        MainWindowViewModel MainWindowViewModel { get; set; }
        ListView ProjectPathsListView { get; }
    }

    public class MainWindowPresenter
    {
        private readonly IMainWindowPresenter presenter;
        private GroupModalWindow groupModalWindow;

        public MainWindowPresenter(IMainWindowPresenter presenter)
        {
            presenter.OpenDevApp += Presenter_OpenDevApp;
            presenter.FetchDevAppsEvent += Presenter_FetchDevAppsEvent;
            presenter.DeleteDevAppEvent += Presenter_DeleteDevAppEvent;
            presenter.NewProjectEvent += Presenter_NewProjectEvent;
            presenter.OpenProjectDialog += Presenter_OpenProjectDialog;
            presenter.DeleteProjectEvent += Presenter_DeleteProjectEvent;
            presenter.SearchProjectEvent += Presenter_SearchProjectEvent;
            presenter.SortUpProjectEvent += Presenter_SortUpProjectEvent;
            presenter.SelectProjectEvent += Presenter_SelectProjectEvent;
            presenter.SortDownProjectEvent += Presenter_SortDownProjectEvent;
            presenter.OpenAddToGroupModalWindowEvent += Presenter_OpenAddToGroupModalWindowEvent;
            presenter.AddProjectToGroupService!.Notify += MainWindowPresenter_Notify;
            presenter.RemoveProjectFromGroupService!.Notify += RemoveProjectFromGroup_Notify;
            presenter.SaveProjectEvent += Presenter_SaveProjectEvent;

            this.presenter = presenter;

            if (this.presenter.MainWindowViewModel is null)
            {
                this.presenter.MainWindowViewModel = new();
            }
            else
            {
                MessageBox.Show("Instance of MainWindowViewModel was created earlier!");
            }
        }

        private async void Presenter_SaveProjectEvent(object? sender, EventArgs e)
        {
            if (this.presenter.MainWindowViewModel!.SelectedProjectPath?.Id == 0)
            {
                var addResult = await this.presenter.AddProjectService!.AddAsync(
                    new(
                        this.presenter.MainWindowViewModel!.SelectedProjectPath!.Name,
                        this.presenter.MainWindowViewModel!.SelectedProjectPath!.Path,
                        this.presenter.MainWindowViewModel!.SelectedIdePath!.Id,
                        this.presenter.MainWindowViewModel!.SelectedProjectPath!.Filename
                    )
                );

                if (addResult)
                {
                    await Search();
                    SelectNewlyAddedItem();
                }
                return;
            }

            var editResult = await this.presenter.EditProjectService!.Edit(
                new(
                    this.presenter.MainWindowViewModel!.SelectedProjectPath!.Id,
                    this.presenter.MainWindowViewModel!.SelectedProjectPath!.Name,
                    this.presenter.MainWindowViewModel!.SelectedProjectPath!.Path,
                    this.presenter.MainWindowViewModel!.SelectedIdePath!.Id,
                    this.presenter.MainWindowViewModel!.SelectedProjectPath!.Filename
                )
            );

            if (editResult)
            {
                await Search();
                SelectEditedItem();
            }
        }

        private void SelectNewlyAddedItem()
        {
            presenter.ProjectPathsListView.SelectedItem = presenter.ProjectPathsListView.Items[^1];
            presenter.ProjectPathsListView.ScrollIntoView(
                this.presenter.ProjectPathsListView.SelectedItem
            );
        }

        private async void RemoveProjectFromGroup_Notify(
            object? sender,
            RemoveProjectFromGroupEventArgs e
        )
        {
            await Search();
            SelectEditedItem();
            groupModalWindow!.Close();
        }

        private async void MainWindowPresenter_Notify(object? sender, EventArgs e)
        {
            await this.Search();
            SelectEditedItem();
            groupModalWindow!.Close();
        }

        private void SelectEditedItem()
        {
            presenter.ProjectPathsListView.SelectedItem = presenter
                .ProjectPathsListView.Items.SourceCollection.Cast<ProjectViewModel>()
                .FirstOrDefault(projectPathsViewModel =>
                    projectPathsViewModel.Id
                    == this.presenter.MainWindowViewModel?.SelectedProjectPath?.Id
                );

            presenter.ProjectPathsListView.ScrollIntoView(
                this.presenter.ProjectPathsListView.SelectedItem
            );
        }

        private void Presenter_OpenAddToGroupModalWindowEvent(object? sender, EventArgs e)
        {
            this.groupModalWindow = new GroupModalWindow(
                presenter.GetAllGroupService!,
                presenter.AddProjectToGroupService!,
                presenter.RemoveProjectFromGroupService!
            );

            this.groupModalWindow!.ProjectPath = this.presenter
                .MainWindowViewModel!
                .SelectedProjectPath;

            groupModalWindow?.ShowDialog();
        }

        private async void Presenter_SortDownProjectEvent(object? sender, EventArgs e)
        {
            var result = await this.presenter.SortDownProjectService!.Handle(
                new() { SortId = this.presenter.MainWindowViewModel!.SelectedProjectPath!.SortId }
            );
            if (!result)
            {
                return;
            }

            await this.Search();
        }

        private void Presenter_SelectProjectEvent(object? sender, EventArgs e)
        {
            if (presenter.ProjectPathsListView.SelectedIndex == -1)
            {
                return;
            }
            var project = (ProjectViewModel)presenter.ProjectPathsListView.SelectedItem;
            var currentGitBranch = presenter.GetCurrentGitBranchService!.Handle(
                new() { DirectoryPath = project.Path }
            );
            project.CurrentGitBranch = currentGitBranch;
            this.presenter.MainWindowViewModel!.SelectedProjectPath = project.Copy();

            // Selected Dev App : Must be same reference, data must be on view model list to set the value
            this.presenter.MainWindowViewModel!.SelectedIdePath =
                this.presenter.MainWindowViewModel!.IdePathsModels!.First(x =>
                    x.Id == project.IDEPathId
                );
        }

        private async void Presenter_SortUpProjectEvent(object? sender, EventArgs e)
        {
            var result = await this.presenter.SortUpProjectService!.Handle(
                new() { SortId = this.presenter.MainWindowViewModel!.SelectedProjectPath!.SortId }
            );

            if (!result)
            {
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
            if (this.presenter.MainWindowViewModel!.SelectedProjectPath!.Id == 0)
            {
                return;
            }

            var result = await this.presenter.DeleteProjectService!.Delete(
                this.presenter.MainWindowViewModel!.SelectedProjectPath!.Id
            );

            if (result)
            {
                await Search();
                this.presenter.MainWindowViewModel!.SelectedProjectPath = new();
                this.presenter.MainWindowViewModel!.SelectedIdePath = new();
            }
        }

        private async Task Search()
        {
            var searchViewModel = await this.presenter.SearchProjectService!.Handle(
                new() { Search = this.presenter.MainWindowViewModel!.Search }
            );
            this.presenter.MainWindowViewModel.EnableAddNewProject =
                searchViewModel.EnableAddNewProject;
            this.presenter.MainWindowViewModel!.ProjectPathModels = [.. searchViewModel.Projects];
        }

        private void Presenter_OpenProjectDialog(object? sender, EventArgs e)
        {
            var openFolderDialog = new OpenFolderDialog();
            var result = openFolderDialog.ShowDialog() ?? false;

            if (result)
            {
                string filePath = openFolderDialog.FolderName;
                string name = openFolderDialog.SafeFolderName;
                var project = this.presenter.MainWindowViewModel!.SelectedProjectPath!.Copy();
                project.Name = name;
                project.Path = filePath;
                this.presenter.MainWindowViewModel!.SelectedProjectPath = project;
            }
        }

        private void Presenter_NewProjectEvent(object? sender, EventArgs e)
        {
            this.presenter.MainWindowViewModel!.SelectedProjectPath = new();
            this.presenter.MainWindowViewModel!.SelectedIdePath = new();
        }

        private async void Presenter_DeleteDevAppEvent(object? sender, EventArgs e)
        {
            if (this.presenter.MainWindowViewModel!.SelectedIdePath!.Id == 0)
                return;

            var result = await this.presenter.DeleteDevAppService!.Delete(
                new() { Id = this.presenter.MainWindowViewModel!.SelectedIdePath!.Id }
            );

            if (result)
            {
                var getAllDevAppVm = await presenter.GetAllDevAppService!.Handle();
                presenter.MainWindowViewModel!.IdePathsModels = [.. getAllDevAppVm.DevApps];
            }
        }

        private async void Presenter_FetchDevAppsEvent(object? sender, EventArgs e)
        {
            var getAllDevAppVm = await presenter.GetAllDevAppService!.Handle();
            presenter.MainWindowViewModel!.IdePathsModels = [.. getAllDevAppVm.DevApps];
        }

        private async void Presenter_OpenDevApp(object? sender, EventArgs e)
        {
            var openFolderDialog = new OpenFileDialog { Filter = "Executable Files | *.exe" };
            var result = openFolderDialog.ShowDialog() ?? false;

            if (!result)
            {
                return;
            }

            presenter.DevAppFilePath = openFolderDialog.FileName;
            var resultSave = await this.presenter.AddDevAppService!.Add(
                new() { Path = this.presenter.DevAppFilePath }
            );
            if (resultSave)
            {
                var getAllDevAppVm = await presenter.GetAllDevAppService!.Handle();
                presenter.MainWindowViewModel!.IdePathsModels = [.. getAllDevAppVm.DevApps];

                presenter.MainWindowViewModel!.SelectedIdePath = new();
            }
        }
    }
}
