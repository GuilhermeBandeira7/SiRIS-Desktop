using EntityMtwServer;
using EntityMtwServer.Entities;
using EntityMtwServer.Services;
using SiRISApp.Services;
using SiRISApp.ViewModel.Commands;
using System;
using System.ComponentModel;
using System.Linq;

namespace SiRISApp.ViewModel
{
    public class LoginVM : INotifyPropertyChanged
    {
        private readonly MasterServerContext _masterServerContext = new();

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
        public event EventHandler? Authenticated;

        public LoginCommand LoginCommand { get; set; }

        public LoginVM()
        {
            LoginCommand = new(this);
        }


        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public async void Login()
        {
            UsersService usersService = new(_masterServerContext);
            Response response = await usersService.GetUserByUsernamePassword(Username, Password);
            if (response.Result && response.Value != null)
            {
                ServerConfig serverConfig = ServerConfigService.Instance.GetServerConfig();
                AppSessionService.Instance.User = (User)response.Value;
                DVC? device = _masterServerContext.DVCs
                    .Where(d => d.SerialNumber == serverConfig.SerialNumber)
                    .FirstOrDefault();
                if (device != null)
                {
                    AppSessionService.Instance.Device = device;
                    Authenticated?.Invoke(this, new());
                }
            }
        }
    }
}
