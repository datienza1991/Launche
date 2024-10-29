using ApplicationCore.Features.DevApps;
using Infrastructure.ViewModels;
using Microsoft.Win32;
using System.Windows;

namespace UI.MainWindowx.Presenter
{
    public interface IMainWindowPresenter
    {
        event EventHandler OpenDevApp;
        event EventHandler FetchDevAppsEvent;
        string DevAppFilePath { get; set; }
        IAddDevAppService? AddDevAppService { get; }
        IGetAllDevAppService? GetAllDevAppService { get; }
        MainWindowViewModel MainWindowViewModel { get; set; }
    }

    public class MainWindowPresenter
    {
        private readonly IMainWindowPresenter presenter;

        public MainWindowPresenter(IMainWindowPresenter presenter)
        {

            presenter.OpenDevApp += Presenter_OpenDevApp;
            presenter.FetchDevAppsEvent += Presenter_FetchDevAppsEvent;
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
