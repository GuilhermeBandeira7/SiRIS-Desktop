using EntityMtwServer.Entities;
using EntityMtwServer.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic.Logging;
using NAudio.Wave;
using SiRISApp.Services;
using SiRISApp.View;
using SiRISApp.View.UserControls.Login;
using SiRISApp.View.Windows.SiRIS;
using SiRISApp.ViewModel.Commands;
using SiRISApp.ViewModel.Login;
using SiRISApp.ViewModel.SiRIS.Login;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;

namespace SiRISApp.ViewModel.Login
{
    public class LoginViewModel : INotifyPropertyChanged
    {
        public enum LOGIN_INDEX
        {
            FORM = 0,
            SERVER_CONFIG = 1,
            DATABASE_CONNECT = 2,
            LOGIN_CHECK = 3,
            LOAD_DATA = 4,
            WELCOME_MESSAGE = 5
        }

        private int index;
        public int Index
        {
            get { return index; }
            set
            {
                index = value;
                OnPropertyChanged(nameof(index));
            }
        }

        private Visibility navigateVisibility = Visibility.Visible;
        public Visibility NavigateVisibility
        {
            get { return navigateVisibility; }
            set
            {
                navigateVisibility = value;
                OnPropertyChanged(nameof(NavigateVisibility));
            }
        }

        private string navigateButtonImage = "Cog";
        public string NavigateButtonImage
        {
            get { return navigateButtonImage; }
            set
            {
                navigateButtonImage = value;
                OnPropertyChanged(nameof(NavigateButtonImage));

            }
        }

        private string selectedCountry = "../../Assets/Flags/brazil-.png";
        public string SelectedCountry
        {
            get { return selectedCountry; }
            set
            {
                var app = (App)Application.Current;

                if (value.Contains("Brasil"))
                {
                    app.ChangeLanguage(new Uri("Translation/StringResources_PT-BR.xaml", UriKind.Relative));
                    selectedCountry = "../../Assets/Flags/brazil-.png";
                }
                else if (value.Contains("US"))
                {
                    app.ChangeLanguage(new Uri("Translation/StringResources_EN.xaml", UriKind.Relative));
                    selectedCountry = "../../Assets/Flags/united-states.png";
                }
                else
                {
                    app.ChangeLanguage(new Uri("Translation/StringResources_PT-BR.xaml", UriKind.Relative));
                    selectedCountry = "../../Assets/Flags/brazil-.png";
                }

                OnPropertyChanged(nameof(SelectedCountry));
            }
        }

        private string selectedLanguage = "Português-Brasil";
        public string SelectedLanguage
        {
            get { return selectedLanguage; }
            set
            {
                selectedLanguage = value;
                OnPropertyChanged(nameof(SelectedLanguage));
                SelectedCountry = value;
            }
        }

        public ObservableCollection<string> Languages { get; set; } = new()
        {
            "Português-Brasil",
            "English-US",
        };


        public event PropertyChangedEventHandler? PropertyChanged;
        public event EventHandler? Authenticated;

        public NavigateCommand NavigateCommand { get; set; }

        public LoginConfigViewModel LoginConfigViewModel { get; set; } = new();
        public LoginFormViewModel LoginFormViewModel { get; set; } = new();

        public LoginViewModel()
        {
            NavigateCommand = new(this);
            LoginFormViewModel.LoginEvent += LoginCallback;
        }

        public void Navigate()
        {
            if (Index == (int)LOGIN_INDEX.FORM)
            {
                Index = (int)LOGIN_INDEX.SERVER_CONFIG;
                NavigateButtonImage = "ArrowBack";

            }
            else if (Index == (int)LOGIN_INDEX.SERVER_CONFIG)
            {
                Index = (int)LOGIN_INDEX.FORM;
                NavigateButtonImage = "Cog";
            }
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void LoginCallback(object? sender, EventArgs e)
        {
            Thread t = new Thread(ConnectToDatabase);
            t.SetApartmentState(ApartmentState.STA);
            t.Start();
        }

        private async void ConnectToDatabase()
        {
            try
            {
                NavigateVisibility = Visibility.Collapsed;
                Index = (int)LOGIN_INDEX.DATABASE_CONNECT;
                Thread.Sleep(1000);
                var users = await AppSessionService.Instance.Context.Users.AsNoTracking().ToListAsync();
                if (users.Count() > 0)
                {
                    Thread t = new Thread(CheckLogin);
                    t.SetApartmentState(ApartmentState.STA);
                    t.Start();
                }
            }
            catch (Exception ex)
            {
                Index = (int)LOGIN_INDEX.FORM;
                NavigateVisibility = Visibility.Visible;
                MessageService.Instance.Show("error", $"{ex.Message}");
            }
        }

        private async void CheckLogin()
        {
            try
            {
                Index = (int)LOGIN_INDEX.LOGIN_CHECK;
                Thread.Sleep(1000);
                UsersService usersService = new(AppSessionService.Instance.Context);
                Response response = await usersService.GetUserByUsernamePassword(LoginFormViewModel.Username, LoginFormViewModel.Password);

                if (response.Result && response.Value != null)
                {
                    AppSessionService.Instance.User = (User)response.Value;
                    OBSService.Instance.Start();
                    while (OBSService.Instance.State != OBS_STATE.CONNECTED) { Thread.Sleep(1000); }
                    FtpService.Instance.Init();
                    Thread t = new Thread(InitSiRIS);
                    t.SetApartmentState(ApartmentState.STA);
                    t.Start();
                }
                else
                {
                    Index = (int)LOGIN_INDEX.FORM;
                    NavigateVisibility = Visibility.Visible;
                    MessageService.Instance.Show("error", "failToLogin");
       
                }
            }
            catch (Exception ex)
            {
                Index = (int)LOGIN_INDEX.FORM;
                NavigateVisibility = Visibility.Visible;
                MessageService.Instance.Show("error", $"{ex.Message}");

            }

        }

        private void InitSiRIS()
        {
            try
            {
                Index = (int)LOGIN_INDEX.LOAD_DATA;
                Thread.Sleep(1000);
                string filePath = Directory.GetCurrentDirectory();
                var reader = new Mp3FileReader($"{filePath}\\View\\Assets\\Audios\\synthesize_female.mp3");
                var waveOut = new WaveOut();
                waveOut.Init(reader);
                waveOut.Play();
                ServerConfig serverConfig = ServerConfigService.Instance.GetServerConfig();
                DVC? device = AppSessionService.Instance.Context.DVCs
                    .Where(d => d.SerialNumber == serverConfig.SerialNumber)
                    .FirstOrDefault();
                if (device != null)
                    AppSessionService.Instance.Device = device;

         
                Index = (int)LOGIN_INDEX.WELCOME_MESSAGE;
                Thread.Sleep(3000);
                Authenticated?.Invoke(this, new EventArgs());
            }
            catch (Exception ex)
            {
                Index = (int)LOGIN_INDEX.FORM;
                NavigateVisibility = Visibility.Visible;
                MessageService.Instance.Show("error", $"{ex.Message}");

            }
        }


    }
}
