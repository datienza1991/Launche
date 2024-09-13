using System.Collections.ObjectModel;
using System.ComponentModel;

namespace UI.Windows.Group.ViewModels
{
    public class GroupWindowDataContext : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private ObservableCollection<GroupViewModel> groups = [];

        public ObservableCollection<GroupViewModel> Groups
        {
            get { return groups; }
            set
            {
                groups = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.Groups)));
            }
        }

        private GroupViewModel? selectedOption = new();

        public GroupViewModel? SelectedOption
        {
            get { return selectedOption; }
            set
            {
                selectedOption = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.SelectedOption)));
            }
        }
    }
}
