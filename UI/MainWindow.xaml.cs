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

        public MainWindow()
        {
            InitializeComponent();
        }

        public MainWindow(IInitializedDatabaseMigration initializedDatabaseMigration, IGetVsCodePath getVsCodePath)
        {
            InitializeComponent();
            initializedDatabaseMigration.Execute();
            this.getVsCodePath = getVsCodePath;
        }

        private void VsCodePathOpenDialogButton_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "All Files (*.*)|*.*"; // Set the filter if needed
            var result = openFileDialog.ShowDialog() ?? false;

            if (result)
            {
                string filePath = openFileDialog.FileName;
                this.VsCodePathTextBox.Text = filePath;
            }
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.getVsCodePath == null) return;
             
            var vsCodePath = await this.getVsCodePath.ExecuteAsync();

            VsCodePathTextBox.Text = vsCodePath.Path;
        }
    }
}