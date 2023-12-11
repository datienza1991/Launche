using Microsoft.Win32;
using System.Windows;
using UI.Database;
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

        public MainWindow()
        {
            InitializeComponent();
        }

        public MainWindow(IInitializedDatabaseMigration initializedDatabaseMigration, IGetVsCodePath getVsCodePath, ISaveVsCodePath saveVsCodePath)
        {
            InitializeComponent();
            initializedDatabaseMigration.Execute();
            this.getVsCodePath = getVsCodePath;
            this.saveVsCodePath = saveVsCodePath;
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
            if (this.getVsCodePath == null) return;

            var vsCodePath = await this.getVsCodePath.ExecuteAsync();

            VsCodePathTextBox.Text = vsCodePath.Path;
        }

        private async void SaveVsCodePathButton_Click(object sender, RoutedEventArgs e)
        {
            var vsCodePath = VsCodePathTextBox?.Text;
            var result = await this.saveVsCodePath!.ExecuteAsync(vsCodePath!);

            if (result) { MessageBox.Show("Vs Code path saved!"); }
        }
    }
}