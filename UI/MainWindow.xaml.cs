using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using UI.Database;
using UI.IDEPath;
using UI.ProjectPath;

namespace UI;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly IGetIDEPath? getVsCodePath;
        private readonly ISaveIDEPath? saveVsCodePath;
        private readonly IGetProjectPaths? getProjectPaths;
        private readonly IAddProjectPath? addProjectPath;
        private readonly IEditProjectPath? editProjectPath;
        private readonly IGetLastProjectPath? getLastProjectPath;
        private readonly IGetIDEPaths? getIDEPaths;
        private readonly IDeleteProjectPath? deleteProjectPath;
        private readonly IDeleteIdePath? deleteIdePath;
        private readonly ISortUpProjectPath? sortUpProjectPath;
        private readonly ISortDownProjectPath? sortDownProjectPath;
        private readonly MainWindowViewModel? mainWindowViewModel;

        public ObservableCollection<ProjectPath.ProjectPath> Entries { get; private set; } = [];

        public MainWindow() => InitializeComponent();

        public MainWindow(
            IInitializedDatabaseMigration initializedDatabaseMigration,
            IGetIDEPath getVsCodePath,
            ISaveIDEPath saveIdePath,
            IGetProjectPaths getProjectPaths,
            IAddProjectPath addProjectPath,
            IEditProjectPath editProjectPath,
            IGetLastProjectPath getLastProjectPath,
            IGetIDEPaths getIDEPaths,
            IDeleteProjectPath deleteProjectPath,
            IDeleteIdePath deleteIdePath,
            ISortUpProjectPath sortUpProjectPath,
            ISortDownProjectPath sortDownProjectPath
        )
        {
            initializedDatabaseMigration.Execute();
            this.getVsCodePath = getVsCodePath;
            this.saveVsCodePath = saveIdePath;
            this.getProjectPaths = getProjectPaths;
            this.addProjectPath = addProjectPath;
            this.editProjectPath = editProjectPath;
            this.getLastProjectPath = getLastProjectPath;
            this.getIDEPaths = getIDEPaths;
            this.deleteProjectPath = deleteProjectPath;
            this.deleteIdePath = deleteIdePath;
            this.sortUpProjectPath = sortUpProjectPath;
            this.sortDownProjectPath = sortDownProjectPath;
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
            if (resultSave)
            {
                this.mainWindowViewModel!.IdePathsModels!.Clear();
                await this.FetchIDEPaths();
                this.mainWindowViewModel.SelectedIdePath = new();
                this.mainWindowViewModel.SelectedProjectPath = new();
                MessageBox.Show("IDE path saved!");
            }
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            await this.FetchProjectPaths();
            await this.FetchIDEPaths();
        }

        private async Task FetchProjectPaths()
        {

            var projectPaths = await this.getProjectPaths!.ExecuteAsync();

            if (this.getVsCodePath == null) return;

            foreach (var item in projectPaths.Select((value, index) => (value, index)))
            {
                var (value, index) = item;
                index++;

                this.mainWindowViewModel!.ProjectPathModels?.Add
                 (
                    new()
                    {
                        Index = index,
                        Id = value.Id,
                        Name = value.Name,
                        Path = value.Path,
                        IDEPathId = value.IDEPathId,
                        SortId = value.SortId,
                        EnableMoveUp = index != 1,
                        EnableMoveDown = index != projectPaths.Count
                    }
                );
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

            var projectPath = (ProjectPathsViewModel)lvProjectPaths.SelectedItem;

            try
            {
                this.mainWindowViewModel!.SelectedIdePath = this.mainWindowViewModel!.IdePathsModels!.Single(x => x.Id == projectPath.IDEPathId);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            this.mainWindowViewModel!.SelectedProjectPath = ProjectPathViewModel.Transform(projectPath);
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
                result = await this.addProjectPath!.ExecuteAsync(this.mainWindowViewModel!.SelectedProjectPath!);
                this.mainWindowViewModel.SelectedProjectPath.Id = (await this.getLastProjectPath!.ExecuteAsync()).Id;
            }
            else
            {
                result = await this.editProjectPath!.ExecuteAsync(this.mainWindowViewModel.SelectedProjectPath);
            }

            if (result)
            {
                this.mainWindowViewModel!.ProjectPathModels!.Clear();
                await this.FetchProjectPaths();

                MessageBox.Show("Project path saved!");
            }
        }

        private void ProjectPathsList_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
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
                    FileName = this.mainWindowViewModel!.SelectedIdePath!.Path,
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

        private void comboIDEPaths_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {

        }

        private async void btnDeleteProjectPath_Click(object sender, RoutedEventArgs e)
        {
            if (this.mainWindowViewModel!.SelectedProjectPath!.Id == 0) return;

            var result = await this.deleteProjectPath!.ExecuteAsync(this.mainWindowViewModel!.SelectedProjectPath!.Id);

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
            await this.sortUpProjectPath!.ExecuteAsync(this.mainWindowViewModel!.SelectedProjectPath!.SortId);
            this.mainWindowViewModel.ProjectPathModels?.Clear();
            await this.FetchProjectPaths();
        }

        private async void mnuMoveDown_Click(object sender, RoutedEventArgs e)
        {
            await this.sortDownProjectPath!.ExecuteAsync(this.mainWindowViewModel!.SelectedProjectPath!.SortId);
            this.mainWindowViewModel.ProjectPathModels?.Clear();
            await this.FetchProjectPaths();
        }
    }

public class MainWindowViewModel : INotifyPropertyChanged
{
    public MainWindowViewModel()
    {
        this.ProjectPathModels = [];
        this.IdePathsModels = [];
        this.SelectedProjectPath = new();
        this.SelectedIdePath = new();
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private ObservableCollection<ProjectPathsViewModel>? projectPathModels;

    public ObservableCollection<ProjectPathsViewModel>? ProjectPathModels
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

    private ProjectPathViewModel? selectedProjectPath;

    public ProjectPathViewModel? SelectedProjectPath
    {
        get { return selectedProjectPath; }
        set
        {
            selectedProjectPath = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.SelectedProjectPath)));
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
}

public class ProjectPathsViewModel : ProjectPath.ProjectPath
{
    public int Index { get; set; }
    public bool EnableMoveUp { get; set; }
    public bool EnableMoveDown { get; set; }
}

public class ProjectPathViewModel : ProjectPath.ProjectPath
{
    public static ProjectPathViewModel Transform(ProjectPath.ProjectPath from)
    {
        return new()
        {
            Id = from.Id,
            IDEPathId = from.IDEPathId,
            Name = from.Name,
            Path = from.Path,
            SortId = from.SortId,
        };
    }

    public static ProjectPath.ProjectPath Transform(ProjectPathViewModel from)
    {
        return new()
        {
            Id = from.Id,
            IDEPathId = from.IDEPathId,
            Name = from.Name,
            Path = from.Path
        };
    }
}

public class IDEPathsViewModel : IDEPath.IDEPath
{
}

