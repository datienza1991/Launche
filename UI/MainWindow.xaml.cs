using Microsoft.Win32;
using System.Windows;
using UI.Database;

namespace UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        

        public MainWindow()
        {
            InitializeComponent();
        }

        public MainWindow(IInitializedDatabaseMigration initializedDatabaseMigration)
        {
            InitializeComponent();
            initializedDatabaseMigration.Execute();
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
    }
}