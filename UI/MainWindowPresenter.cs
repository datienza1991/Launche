using ApplicationCore.Features.DevApps;
using ApplicationCore.Features.Projects;
using Infrastructure.ViewModels;
using Microsoft.Win32;
using System.Windows;

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
        string DevAppFilePath { get; set; }
        IAddDevAppService? AddDevAppService { get; }
        IGetAllDevAppService? GetAllDevAppService { get; }
        IDeleteDevAppService? DeleteDevAppService { get; }
        IDeleteProjectService? DeleteProjectService { get; }
        MainWindowViewModel MainWindowViewModel { get; set; }
    }

    public class MainWindowPresenter
    {
        private readonly IMainWindowPresenter presenter;

        public MainWindowPresenter(IMainWindowPresenter presenter)
        {

            presenter.OpenDevApp += Presenter_OpenDevApp;
            presenter.FetchDevAppsEvent += Presenter_FetchDevAppsEvent;
            presenter.DeleteDevAppEvent += Presenter_DeleteDevAppEvent;
            presenter.NewProjectEvent += Presenter_NewProjectEvent;
            presenter.OpenProjectDialog += Presenter_OpenProjectDialog;
            presenter.DeleteProjectEvent += Presenter_DeleteProjectEvent;
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

        private async void Presenter_DeleteProjectEvent(object? sender, EventArgs e)
        {
            if (this.presenter.MainWindowViewModel!.SelectedProjectPath!.Id == 0) return;

            var result = await this.presenter.DeleteProjectService!.Delete(this.presenter.MainWindowViewModel!.SelectedProjectPath!.Id);

            if (result)
            {
                //await Search();
                this.presenter.MainWindowViewModel!.SelectedProjectPath = new();
                this.presenter.MainWindowViewModel!.SelectedIdePath = new();
            }
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
            if (this.presenter.MainWindowViewModel!.SelectedIdePath!.Id == 0) return;

            var result = await this.presenter.DeleteDevAppService!.Delete(new() { Id = this.presenter.MainWindowViewModel!.SelectedIdePath!.Id });

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
            var openFolderDialog = new OpenFileDialog
            {
                Filter = "Executable Files | *.exe"
            };
            var result = openFolderDialog.ShowDialog() ?? false;

            if (!result)
            {
                return;
            }

            presenter.DevAppFilePath = openFolderDialog.FileName;
            var resultSave = await this.presenter.AddDevAppService!.Add(new() { Path = this.presenter.DevAppFilePath });
            if (resultSave)
            {

                var getAllDevAppVm = await presenter.GetAllDevAppService!.Handle();
                presenter.MainWindowViewModel!.IdePathsModels = [.. getAllDevAppVm.DevApps];

                presenter.MainWindowViewModel!.SelectedIdePath = new();
            }
        }
    }
}
