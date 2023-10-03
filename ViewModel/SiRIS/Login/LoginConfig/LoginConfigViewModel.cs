using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.Collections.ObjectModel;
using SiRISApp.Services;

namespace SiRISApp.ViewModel.Login
{
    public class LoginConfigViewModel : INotifyPropertyChanged
    {
        private string serialNumber = string.Empty;
        public string SerialNumber
        {
            get { return serialNumber; }
            set
            {
                serialNumber = value;
                OnPropertyChanged(nameof(SerialNumber));
            }
        }

        private string selectedServer = string.Empty;
        public string SelectedServer
        {
            get
            {
                return selectedServer;
            }
            set
            {
                selectedServer = value;
                OnPropertyChanged(nameof(SelectedServer));
            }
        }

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

        private string newServerIp = string.Empty;
        public string NewServerIp
        {
            get { return newServerIp; }
            set
            {
                newServerIp = value;
                OnPropertyChanged(nameof(newServerIp));
            }
        }

        private string newServerName = string.Empty;
        public string NewServerName
        {
            get { return newServerName; }
            set
            {
                newServerName = value;
                OnPropertyChanged(nameof(NewServerName));
            }
        }

        private int selectedIndex;
        public int SelectedIndex
        {
            get { return selectedIndex; }
            set
            {
                selectedIndex = value;
                OnPropertyChanged(nameof(SelectedIndex));
            }
        }

        public ObservableCollection<string> Servers { get; set; } = new();
        public Dictionary<string, string> ServerNames { get; set; } = new();

        public event PropertyChangedEventHandler? PropertyChanged;

        public SwitchServerCommand SwitchServerCommand { get; set; }
        public ValidateLoginCommand ValidateLoginCommand { get; set; }
        public CreateServerCommand CreateServerCommand { get; set; }


        public LoginConfigViewModel()
        {
            SwitchServerCommand = new(this);
            ValidateLoginCommand = new(this);
            CreateServerCommand = new(this);

            ServerNames = ServerConfigService.Instance.GetServers();
            foreach (var server in ServerNames)
                Servers.Add(server.Value);


            ServerConfig config = ServerConfigService.Instance.GetServerConfig();
            serialNumber = config.SerialNumber;
            SelectedServer = ServerNames[config.Ip];
        }

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        public void CreateServer()
        {
            ServerConfigService.Instance.AddServer(newServerIp, newServerName);
            ServerNames.Clear();
            Servers.Clear();
            ServerNames = ServerConfigService.Instance.GetServers();
            foreach (var server in ServerNames)
                Servers.Add(server.Value);
        }

        public void SwitchServer()
        {
            var app = (App)System.Windows.Application.Current;
            string? text = app.LanguageDictionary["serverSelect"].ToString();
            if (text != null)
                text = text.Replace("{0}", selectedServer);
            else
                text = string.Empty;


            bool? result = MessageService.Instance.ShowDialog("warning", text, true);
            if (result != null && result == true)
            {
                ServerConfig serverConfig = ServerConfigService.Instance.GetServerConfig();
                serverConfig.SerialNumber = serialNumber;
                serverConfig.Ip = ServerNames.Where(s => s.Value == SelectedServer).First().Key;
                ServerConfigService.Instance.SetServerConfig(serverConfig);
                ServerConfigService.Instance.SetFtpConfig(serverConfig.Ip, "mtw", "Senha@mtw");

                MessageService.Instance.Show("success", "serverChanged");
            }
        }
    }
}
