using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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

        private void txtVsCodePath_GotFocus(object sender, RoutedEventArgs e)
        {
            if (txtVsCodePath.Text == "Enter some text here...")
            {
                txtVsCodePath.Text = "";
            }
        }

        private void txtVsCodePath_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtVsCodePath.Text))
            {
                txtVsCodePath.Text = "Enter some text here...";
            }
        }
    }
}