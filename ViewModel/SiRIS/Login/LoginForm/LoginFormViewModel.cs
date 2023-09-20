using SiRISApp.ViewModel.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SiRISApp.ViewModel.Login
{
    public class LoginFormViewModel : INotifyPropertyChanged
    {
        private string username = string.Empty;
        public string Username
        {
            get { return username; }
            set
            {
                username = value;
                OnPropertyChanged(nameof(Username));
            }
        }

        private string password = string.Empty;
        public string Password
        {
            get { return password; }
            set
            {
                password = value;
                OnPropertyChanged(nameof(Password));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public LoginCommand LoginCommand { get; set; }
        public EventHandler? LoginEvent { get; set; }

        public LoginFormViewModel()
        {
            LoginCommand = new(this);
        }

        public void Login()
        {
            LoginEvent?.Invoke(this, new EventArgs());
        
        }

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));  
        }
    }
}
