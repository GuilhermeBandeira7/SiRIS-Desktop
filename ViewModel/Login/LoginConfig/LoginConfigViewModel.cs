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
                OnPropertyChanged(SelectedServer);
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

        public UpdateServerConfigCommand UpdateServerConfigCommand { get; set; }
        public ShowAdminLoginCommand ShowAdminLoginCommand { get; set; }
        public ShowCreateServerCommand ShowCreateServerCommand { get; set; }
        public CreateServerCommand CreateServerCommand { get; set; }

        public LoginConfigViewModel()
        {
            UpdateServerConfigCommand = new(this);
            ShowAdminLoginCommand = new(this);
            ShowCreateServerCommand = new(this);
            CreateServerCommand = new(this);

            if (System.IO.File.Exists("servers.txt"))
            {
                List<string> lines = System.IO.File.ReadAllLines("servers.txt").ToList();
                foreach (string line in lines)
                {
                    List<string> lineValues = line.Split(":").ToList();
                    ServerNames.Add(lineValues.First(), lineValues.Last());
                    Servers.Add(lineValues.Last());
                }
            }
            else
            {
                System.IO.File.WriteAllLines("servers.txt", new string[] { "localhost:local" });
            }

            ServerConfig config = ServerConfigService.Instance.GetServerConfig();
            serialNumber = config.SerialNumber;
            SelectedServer = ServerNames[config.Ip];
        }

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void SaveConfig()
        {
            ServerConfig serverConfig = ServerConfigService.Instance.GetServerConfig();
            serverConfig.SerialNumber = serialNumber;
            serverConfig.Ip = ServerNames.Where(s => s.Value == SelectedServer).First().Key;
            ServerConfigService.Instance.SetServerConfig(serverConfig);
        }

        public void CreateServer()
        {
            if (!Servers.Contains(NewServerName))
            {
                List<string> lines = System.IO.File.ReadAllLines("servers.txt").ToList();
                lines.Add($"{NewServerIp}:{NewServerName}");
                System.IO.File.WriteAllLines("servers.txt", lines.ToArray());
            }

            ServerNames.Clear();
            Servers.Clear();

            if (System.IO.File.Exists("servers.txt"))
            {
                List<string> lines = System.IO.File.ReadAllLines("servers.txt").ToList();
                foreach (string line in lines)
                {
                    List<string> lineValues = line.Split(":").ToList();
                    ServerNames.Add(lineValues.First(), lineValues.Last());
                    Servers.Add(lineValues.Last());
                }
            }
        }
    }
}
