using EntityMtwServer.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiRISApp.ViewModel.SessionManagement
{
    public class UserViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private bool selecionado;
        public bool Selecionado
        {
            get { return selecionado; }
            set
            {
                selecionado = value;
                OnPropertyChanged(nameof(selecionado));
            }
        }

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



        private string nome = string.Empty;
        public string Nome
        {
            get { return nome; }
            set
            {
                nome = value;
                OnPropertyChanged(nameof(Nome));
            }
        }

        private string matricula = string.Empty;
        public string Matricula
        {
            get { return matricula; }
            set
            {
                matricula = value;
                OnPropertyChanged(nameof(Matricula));
            }
        }

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public UserViewModel()
        {

        }

        public UserViewModel(User user)
        {
            Id = user.Id;
            Nome = user.Name;
            Matricula = user.Registration;
        }

    }
}
