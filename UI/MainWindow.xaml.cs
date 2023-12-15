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
            IGetProjectPaths getProjectPaths)
        {
            initializedDatabaseMigration.Execute();
            this.getVsCodePath = getVsCodePath;
            this.saveVsCodePath = saveVsCodePath;
            this.getProjectPaths = getProjectPaths;

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

            var projectPaths = await this.getProjectPaths!.ExecuteAsync();

            if (this.getVsCodePath == null) return;

            var vsCodePath = await this.getVsCodePath.ExecuteAsync();
            this.mainWindowViewModel!.VsCodePath = vsCodePath.Path;

            foreach (var item in projectPaths.Select((value, index) => (value, index)))
            {
                var (value, index) = item;
                index++;

                this.mainWindowViewModel.ProjectPathModels?.Add(new() { Id = index, Name = value.Name, Path = value.Path });
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


            var projectPath = (ProjectPathModel)lvProjectPaths.SelectedItem;
            this.mainWindowViewModel!.ProjectPathModel = projectPath!.Path;
            this.mainWindowViewModel.ProjectPathTitle = projectPath!.Name;

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

        private string? projectPathModel;

        public string? ProjectPathModel
        {
            get { return projectPathModel; }
            set
            {
                projectPathModel = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.ProjectPathModel)));
            }

        }

        private string? projectPathTitle;

        public string? ProjectPathTitle
        {
            get { return projectPathTitle; }
            set { projectPathTitle = value; 
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.ProjectPathTitle))); }
        }

    }
