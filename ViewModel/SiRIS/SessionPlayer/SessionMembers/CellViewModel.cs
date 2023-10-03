using EntityMtwServer.Entities;
using SiRISApp.ViewModel.SiRIS.SessionManagement.User;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace SiRISApp.ViewModel.SessionPlayer
{
    public class CellViewModel : INotifyPropertyChanged
    {
        private long id;
        public long Id
        {
            get { return id; }
            set { id = value;
                OnPropertyChanged(nameof(Id));
            }
        }

        private string name = string.Empty;
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        private string playerName = string.Empty;
        public string PlayerName
        {
            get { return playerName; }
            set { playerName = value; }
        }


        private bool requisition = false;
        public bool Requisition
        {
            get
            {
                return requisition;
            }
            set
            {
                requisition = value;
                OnPropertyChanged(nameof(Requisition));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public ObservableCollection<UserViewModel> UsersInsideSession { get; set; } = new();
        public ObservableCollection<UserViewModel> UsesOutsideSession { get; set; } = new();

        public DVC? Dvc { get; set; }

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
