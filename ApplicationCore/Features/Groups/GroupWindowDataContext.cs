using ApplicationCore.Features.Groups;
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
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Groups)));
            }
        }

        private GroupViewModel? selectedOption = new();

        public GroupViewModel? SelectedOption
        {
            get { return selectedOption; }
            set
            {
                selectedOption = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedOption)));
            }
        }

        private bool enableSave = true;

        public bool EnableSave
        {
            get { return enableSave; }
            set
            {
                enableSave = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(EnableSave)));
            }
        }

        private bool enableReset = true;

        public bool EnableReset
        {
            get { return enableReset; }
            set
            {
                enableReset = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(EnableReset)));
            }
        }
    }
}
