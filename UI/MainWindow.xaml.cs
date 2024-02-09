using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using UI.Database;
using UI.IDEPath;
using UI.ProjectPath;

namespace UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly IGetIDEPath? getVsCodePath;
        private readonly ISaveIDEPath? saveVsCodePath;
        private readonly IGetProjectPaths? getProjectPaths;
        private readonly IAddProjectPath addProjectPath;
        private readonly IEditProjectPath editProjectPath;
        private readonly IGetIDEPaths getIDEPaths;
        private readonly MainWindowViewModel? mainWindowViewModel;

        public ObservableCollection<ProjectPath.ProjectPath> Entries { get; private set; } = new ObservableCollection<ProjectPath.ProjectPath>();

        public MainWindow() => InitializeComponent();

        public MainWindow(
            IInitializedDatabaseMigration initializedDatabaseMigration,
            IGetIDEPath getVsCodePath,
            ISaveIDEPath saveIdePath,
            IGetProjectPaths getProjectPaths,
            IAddProjectPath addProjectPath,
            IEditProjectPath editProjectPath,
            IGetIDEPaths getIDEPaths
        )
        {
            initializedDatabaseMigration.Execute();
            this.getVsCodePath = getVsCodePath;
            this.saveVsCodePath = saveIdePath;
            this.getProjectPaths = getProjectPaths;
            this.addProjectPath = addProjectPath;
            this.editProjectPath = editProjectPath;
            this.getIDEPaths = getIDEPaths;
            this.mainWindowViewModel = new MainWindowViewModel();
            DataContext = this.mainWindowViewModel;
            InitializeComponent();

        }


        private async void VsCodePathOpenDialogButton_Click(object sender, RoutedEventArgs e)
        {
            var openFolderDialog = new OpenFileDialog();
            openFolderDialog.Filter = "Executable Files | *.exe";
            var result = openFolderDialog.ShowDialog() ?? false;

            if (!result)
            {
                return;
            }

            string filePath = openFolderDialog.FileName;
            var resultSave = await this.saveVsCodePath!.ExecuteAsync(filePath);
            if (resultSave) { MessageBox.Show("IDE path saved!"); }
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //var vsCodePath = await this.getVsCodePath!.ExecuteAsync();
            //this.mainWindowViewModel!.VsCodePath = vsCodePath.Path;

            await this.FetchProjectPaths();
            await this.FetchIDEPaths();
            this.mainWindowViewModel!.AllowedTypes!.Add("Folder");
            this.mainWindowViewModel!.AllowedTypes!.Add("File");

        }

        private async Task FetchProjectPaths()
        {

            var projectPaths = await this.getProjectPaths!.ExecuteAsync();

            if (this.getVsCodePath == null) return;

            foreach (var item in projectPaths.Select((value, index) => (value, index)))
            {
                var (value, index) = item;
                index++;

                this.mainWindowViewModel!.ProjectPathModels?.Add(new() { Index = index, Id = value.Id, Name = value.Name, Path = value.Path, IDEPathId = value.IDEPathId });
            }
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
            if (lvProjectPaths.SelectedIndex == -1) return;

            var projectPath = (ProjectPath.ProjectPath)lvProjectPaths.SelectedItem;

            this.mainWindowViewModel!.ProjectPath = projectPath!.Path;
            this.mainWindowViewModel.ProjectPathTitle = projectPath!.Name;
            this.mainWindowViewModel.ProjectPathId = projectPath.Id;
            this.mainWindowViewModel.SelectedIdePath = this.mainWindowViewModel!.IdePathsModels!.Single(x => x.Id == projectPath.IDEPathId);

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
            var idePath = this.mainWindowViewModel!.IdePathsModels!.Where((item, index) => index == this.comboIDEPaths.SelectedIndex).Single();
            var id = this.mainWindowViewModel?.ProjectPathId;

            bool result;

            if (this.mainWindowViewModel!.ProjectPathId == 0)
            {

                result = await this.addProjectPath!.ExecuteAsync(new() { Path = projectPath!, Name = projectPathName!, IDEPathId =  idePath.Id});
            }
            else
            {
                result = await this.editProjectPath!.ExecuteAsync(new() { Id = id!.Value, Path = projectPath!, Name = projectPathName!, IDEPathId = idePath.Id });
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

        private void ProjectPathsList_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            try
            {
                if (!Directory.Exists(this.mainWindowViewModel!.ProjectPath))
                {
                    MessageBox.Show("Directory not found!", "Launch IDE Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                ProcessStartInfo startInfo = new()
                {
                    //FileName = this.mainWindowViewModel!.VsCodePath,
                    Arguments = this.mainWindowViewModel!.ProjectPath,
                };
                Process.Start(startInfo);

            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message, "Launch IDE Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private async void btnDeleteIdePath_Click(object sender, RoutedEventArgs e)
        {
            
        }
    }
}

public class MainWindowViewModel : INotifyPropertyChanged
{
    public MainWindowViewModel()
    {
        this.ProjectPathModels = new();
        this.IdePathsModels = new();
        this.AllowedTypes = new();
    }

    public event PropertyChangedEventHandler? PropertyChanged;

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

    private ObservableCollection<IDEPathsViewModel>? idePathsModels;

    public ObservableCollection<IDEPathsViewModel>? IdePathsModels
    {
        get { return idePathsModels; }
        set
        {
            idePathsModels = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.IdePathsModels)));
        }
    }

    private ObservableCollection<string>? allowedTypes;

    public ObservableCollection<string>? AllowedTypes
    {
        get { return allowedTypes; }
        set
        {
            allowedTypes = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.AllowedTypes)));
        }
    }

    private string? selectedAllowedType;

    public string? SelectedAllowedType
    {
        get { return selectedAllowedType; }
        set
        {
            selectedAllowedType = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.SelectedAllowedType)));
        }
    }

    private IDEPathsViewModel? selectedIdePath;

    public IDEPathsViewModel? SelectedIdePath
    {
        get { return selectedIdePath; }
        set
        {
            selectedIdePath = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.SelectedIdePath)));
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

public class IDEPathsViewModel : IDEPath
{
}
