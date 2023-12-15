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
        private readonly IEditProjectPath editProjectPath;
        private readonly MainWindowViewModel? mainWindowViewModel;

        public ObservableCollection<ProjectPath.ProjectPath> Entries { get; private set; } = new ObservableCollection<ProjectPath.ProjectPath>();

        public MainWindow()
        {
            InitializeComponent();
        }

        public MainWindow(
            IInitializedDatabaseMigration initializedDatabaseMigration,
            IGetVsCodePath getVsCodePath,
            ISaveVsCodePath saveVsCodePath,
            IGetProjectPaths getProjectPaths,
            IAddProjectPath addProjectPath,
            IEditProjectPath editProjectPath
        )
        {
            initializedDatabaseMigration.Execute();
            this.getVsCodePath = getVsCodePath;
            this.saveVsCodePath = saveVsCodePath;
            this.getProjectPaths = getProjectPaths;
            this.addProjectPath = addProjectPath;
            this.editProjectPath = editProjectPath;
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

                this.mainWindowViewModel!.ProjectPathModels?.Add(new() { Index = index, Id = value.Id, Name = value.Name, Path = value.Path });
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

            var projectPath = (ProjectPath.ProjectPath)lvProjectPaths.SelectedItem;

            this.mainWindowViewModel!.ProjectPath = projectPath!.Path;
            this.mainWindowViewModel.ProjectPathTitle = projectPath!.Name;
            this.mainWindowViewModel.ProjectPathId = projectPath.Id;
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
            var id = this.mainWindowViewModel?.ProjectPathId;

            bool result;

            if (this.mainWindowViewModel!.ProjectPathId == 0)
            {

                result = await this.addProjectPath!.ExecuteAsync(new() { Path = projectPath!, Name = projectPathName! });
            }
            else
            {
                result = await this.editProjectPath!.ExecuteAsync(new() { Id = id!.Value, Path = projectPath!, Name = projectPathName! });
            }

            if (result)
            {
                this.mainWindowViewModel.ProjectPathModels!.Clear();
                await this.FetchProjectPaths();
                this.mainWindowViewModel!.ProjectPath = "";
                this.mainWindowViewModel!.ProjectPathId = 0;
                this.mainWindowViewModel!.ProjectPathTitle = "";
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

    private ObservableCollection<ProjectPathViewModel>? projectPathModels;

    public ObservableCollection<ProjectPathViewModel>? ProjectPathModels
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

    public int ProjectPathId { get; internal set; }
}

public class ProjectPathViewModel : ProjectPath
{
    public int Index { get; set; }


}
