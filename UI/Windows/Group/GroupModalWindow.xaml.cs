using System.Windows;
using UI.Windows.Group.ViewModels;

namespace UI
{
    /// <summary>
    /// Interaction logic for GroupModalWindow.xaml
    /// </summary>
    public partial class GroupModalWindow : Window
    {
        private readonly GroupWindowDataContext dataContext = new();

        public GroupModalWindow()
        {
            InitializeComponent();
            DataContext = dataContext;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            GroupViewModel item2 = new() { Name = "Item2" };
            dataContext.Groups = [new() { Name = "Name" }, item2];
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(dataContext.SelectedOption.Name);
        }
    }
}
