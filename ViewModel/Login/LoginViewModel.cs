using EntityMtwServer.Entities;
using EntityMtwServer.Services;
using Microsoft.EntityFrameworkCore;
using NAudio.Wave;
using SiRISApp.Services;
using SiRISApp.View.Windows.SiRIS;
using SiRISApp.ViewModel.Commands;
using SiRISApp.ViewModel.Login;
using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;

namespace SiRISApp.ViewModel.Login
{
    public class LoginViewModel : INotifyPropertyChanged
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

        private int loginIndex;
        public int LoginIndex
        {
            get { return loginIndex; }
            set
            {
                loginIndex = value;
                OnPropertyChanged(nameof(LoginIndex));
            }
        }


        private int loginState;
        public int LoginState
        {
            get { return loginState; }
            set
            {
                loginState = value;
                OnPropertyChanged(nameof(LoginState));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        public event EventHandler? Authenticated;

        public LoginCommand LoginCommand { get; set; }
        public LoginConfigViewModel LoginConfig { get; set; } = new();


        public LoginViewModel()
        {
            LoginCommand = new(this);
        }


        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private async void ConnectToDatabase()
        {
            try
            {
                var users = await AppSessionService.Instance.Context.Users.AsNoTracking().ToListAsync();
                if (users.Count() > 0)
                {
                    Thread.Sleep(5000);
                    LoginState += 1;
                    Thread t = new Thread(CheckLogin);
                    t.SetApartmentState(ApartmentState.STA);

                    t.Start();
                }
            }
            catch (Exception ex)
            {
                LoginIndex = 0;
                LoginState = 0;
                Application.Current.Dispatcher.Invoke((Action)delegate
                {
                    Message message = new();
                    message.SetType("error", $"FALHA NA CONEXÃO: {ex.Message}");
                    message.ShowDialog();
                });


            }
        }

        private async void CheckLogin()
        {
            UsersService usersService = new(AppSessionService.Instance.Context);
            Response response = await usersService.GetUserByUsernamePassword(Username, Password);

            if (response.Result && response.Value != null)
            {
                Thread.Sleep(5000);
                LoginState += 1;
                AppSessionService.Instance.User = (User)response.Value;

                OBSService.Instance.Start();
                while (OBSService.Instance.State != OBS_STATE.CONNECTED)
                {
                    Thread.Sleep(1000);
                }

                FtpService.Instance.Init();

                Thread t = new Thread(InitSiRIS);
                t.SetApartmentState(ApartmentState.STA);

                t.Start();

            }
            else
            {
                LoginIndex = 0;
                LoginState = 0;
                Application.Current.Dispatcher.Invoke((Action)delegate
                {
                    Message message = new();
                    message.SetType("error", "Usuário ou senha inválidos, para recueperação de login por favor entre em contato com o suporte da MTW");
                    message.ShowDialog();
                });


            }
        }

        private void InitSiRIS()
        {
            Thread.Sleep(5000);
            string filePath = Directory.GetCurrentDirectory();
            LoginState += 1;
            var reader = new Mp3FileReader($"{filePath}\\View\\Assets\\synthesize_female.mp3");
            var waveOut = new WaveOut();
            waveOut.Init(reader);
            waveOut.Play();
            ServerConfig serverConfig = ServerConfigService.Instance.GetServerConfig();
            DVC? device = AppSessionService.Instance.Context.DVCs
                .Where(d => d.SerialNumber == serverConfig.SerialNumber)
                .FirstOrDefault();
            if (device != null)
                AppSessionService.Instance.Device = device;

            Thread.Sleep(5000);
            Authenticated?.Invoke(this, new EventArgs());


        }

        public void Login()
        {
            Thread t = new Thread(ConnectToDatabase);
            t.SetApartmentState(ApartmentState.STA);

            t.Start();

        }
    }
}
