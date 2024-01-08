using EntityMtwServer.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SiRISApp.ViewModel.SiRIS.SessionPlayer.SessionMembers.Cell
{
    public class CellMemberViewModel : INotifyPropertyChanged
    {
        private long id;
        public long Id
        {
            get { return id; }
            set
            {
                id = value;
                OnPropertyChanged(nameof(Id));
            }
        }

        private string name = string.Empty;
        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                OnPropertyChanged(nameof(Name));

            }
        }


        public RemoveUserFromSessionCommand RemoveUserFromSessionCommand { get; set; }

        public CellMemberViewModel(User users)
        {
            Id = users.Id;
            Name = users.Name;
            RemoveUserFromSessionCommand = new(this);
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
