using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using UI.Database;
using UI.ProjectPath;
using UI.VsCodePath;

namespace UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly IGetVsCodePath? getVsCodePath;
        private readonly ISaveVsCodePath? saveVsCodePath;
        private readonly IGetProjectPaths? getProjectPaths;
        private readonly IAddProjectPath addProjectPath;
        private readonly MainWindowViewModel? mainWindowViewModel;

        public ObservableCollection<ProjectPathModel> Entries { get; private set; } = new ObservableCollection<ProjectPathModel>();

        public MainWindow()
        {
            InitializeComponent();
        }

        public MainWindow(
            IInitializedDatabaseMigration initializedDatabaseMigration,
            IGetVsCodePath getVsCodePath,
            ISaveVsCodePath saveVsCodePath,
            IGetProjectPaths getProjectPaths,
            IAddProjectPath addProjectPath
        )
        {
            initializedDatabaseMigration.Execute();
            this.getVsCodePath = getVsCodePath;
            this.saveVsCodePath = saveVsCodePath;
            this.getProjectPaths = getProjectPaths;
            this.addProjectPath = addProjectPath;
            this.mainWindowViewModel = new MainWindowViewModel();
            DataContext = this.mainWindowViewModel;
            InitializeComponent();

        }


        private void VsCodePathOpenDialogButton_Click(object sender, RoutedEventArgs e)
        {
            var openFolderDialog = new OpenFolderDialog();
            var result = openFolderDialog.ShowDialog() ?? false;

            if (result)
            {
                string filePath = openFolderDialog.FolderName;
                this.mainWindowViewModel!.VsCodePath = filePath;
            }
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var vsCodePath = await this.getVsCodePath!.ExecuteAsync();
            this.mainWindowViewModel!.VsCodePath = vsCodePath.Path;

            await this.FetchProjectPaths();

        }

        private async Task FetchProjectPaths()
        {

            var projectPaths = await this.getProjectPaths!.ExecuteAsync();

            if (this.getVsCodePath == null) return;

            foreach (var item in projectPaths.Select((value, index) => (value, index)))
            {
                var (value, index) = item;
                index++;

                this.mainWindowViewModel!.ProjectPathModels?.Add(new() { Id = index, Name = value.Name, Path = value.Path });
            }
        }

        private async void SaveVsCodePathButton_Click(object sender, RoutedEventArgs e)
        {
            var vsCodePath = VsCodePathTextBox?.Text;
            var result = await this.saveVsCodePath!.ExecuteAsync(vsCodePath!);

            if (result) { MessageBox.Show("Vs Code path saved!"); }
        }

        private void ProjectPathsList_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (lvProjectPaths.SelectedIndex == -1) return;

            var projectPath = (ProjectPathModel)lvProjectPaths.SelectedItem;

            this.mainWindowViewModel!.ProjectPath = projectPath!.Path;
            this.mainWindowViewModel.ProjectPathTitle = projectPath!.Name;
        }

        private void btnOpenDialogProjectPath_Click(object sender, RoutedEventArgs e)
        {
            var openFolderDialog = new OpenFolderDialog();
            var result = openFolderDialog.ShowDialog() ?? false;

            if (result)
            {
                string filePath = openFolderDialog.FolderName;
                string name = openFolderDialog.SafeFolderName;

                this.mainWindowViewModel!.ProjectPath = filePath!;
                this.mainWindowViewModel.ProjectPathTitle = name;
            }
        }

        private async void btnSaveProjectPath_Click(object sender, RoutedEventArgs e)
        {
            var projectPathName = this.mainWindowViewModel?.ProjectPathTitle;
            var projectPath = this.mainWindowViewModel?.ProjectPath;
            var result = await this.addProjectPath!.ExecuteAsync(new() { Path = projectPath!, Name = projectPathName! });

            if (result)
            {
                this.mainWindowViewModel?.ProjectPathModels!.Clear();
                await this.FetchProjectPaths();

                MessageBox.Show("Project path saved!");
            }
        }
    }
}

public class MainWindowViewModel : INotifyPropertyChanged
{
    public MainWindowViewModel()
    {
        this.ProjectPathModels = new();
    }

    private string? vsCodePath;

    public event PropertyChangedEventHandler? PropertyChanged;

    public string? VsCodePath
    {
        get { return vsCodePath; }
        set
        {
            vsCodePath = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("VsCodePath"));
        }
    }

    private ObservableCollection<ProjectPathModel>? projectPathModels;

    public ObservableCollection<ProjectPathModel>? ProjectPathModels
    {
        get { return projectPathModels; }
        set
        {
            projectPathModels = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.ProjectPathModels)));
        }
    }

    private string? projectPath;

    public string? ProjectPath
    {
        get { return projectPath; }
        set
        {
            projectPath = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.ProjectPath)));
        }

    }

    private string? projectPathTitle;

    public string? ProjectPathTitle
    {
        get { return projectPathTitle; }
        set
        {
            projectPathTitle = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.ProjectPathTitle)));
        }
    }

}
