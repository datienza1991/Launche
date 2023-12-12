using Microsoft.Win32;
using System.Collections.ObjectModel;
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
            DataContext = this;
         
            InitializeComponent();

        }


        private void VsCodePathOpenDialogButton_Click(object sender, RoutedEventArgs e)
        {
            var openFolderDialog = new OpenFolderDialog();
            var result = openFolderDialog.ShowDialog() ?? false;

            if (result)
            {
                string filePath = openFolderDialog.FolderName;
                this.VsCodePathTextBox.Text = filePath;
            }
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var projectPaths = await this.getProjectPaths!.ExecuteAsync();

            if (this.getVsCodePath == null) return;

            var vsCodePath = await this.getVsCodePath.ExecuteAsync();

            VsCodePathTextBox.Text = vsCodePath.Path;

            foreach (var item in projectPaths.Select((value, index) => (value, index)))
            {
                var (value, index) = item;
                index++;

                Entries.Add(new() { Id = index, Name = value.Name, Path= value.Path });
            }
        }

        private async void SaveVsCodePathButton_Click(object sender, RoutedEventArgs e)
        {
            var vsCodePath = VsCodePathTextBox?.Text;
            var result = await this.saveVsCodePath!.ExecuteAsync(vsCodePath!);

            if (result) { MessageBox.Show("Vs Code path saved!"); }
        }
    }
}