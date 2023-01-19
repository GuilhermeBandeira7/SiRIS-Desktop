using EntityMtwServer.Entities;
using MTWServerApiClient;
using SiRISApp.ViewModel.Commands;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace SiRISApp.ViewModel
{
    public class SiRISVm : INotifyPropertyChanged
    {

        public ObservableCollection<Session> Sessions { get; set; }

        private Session runningSession;
        public Session RunningSession
        {
            get { return runningSession; }
            set
            {
                runningSession = value;
                runningSession.Active = true;
                foreach(var session in Sessions)
                    if(session.Id != runningSession.Id)
                        session.Active = false;
                OnPropertyChanged("RunningSession");
            }
        }

        private Session selectedSession;
        public Session SelectedSession
        {
            get { return selectedSession; }
            set
            {
                selectedSession = value;
                OnPropertyChanged("SelectedSession");
            }
        }

        private User user = new User();
        public User User
        {
            get
            {
                return user;
            }
            set
            {
                user = value;
                GetSessions();
                OnPropertyChanged("User");
            }
        }


        private string username;
        public string Username
        {
            get { return username; }
            set
            {
                username = value;
                User = new User()
                {
                    Login = username,
                    Password = this.Password
                };

                OnPropertyChanged("Username");
            }
        }


        private string password;
        public string Password
        {
            get { return password; }
            set
            {
                password = value;
                User = new User()
                {
                    Login = this.Username,
                    Password = password
                };

                OnPropertyChanged("Password");
            }
        }

        public LoginCommand LoginCommand { get; set; }
        public RefreshSessionsCommand RefreshSessionsCommand { get; set; }
        public StartSessionCommand StartSessionCommand { get; set; }


        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler Authenticated;

        public SiRISVm()
        {
            LoginCommand = new LoginCommand(this);
            RefreshSessionsCommand = new RefreshSessionsCommand(this);
            StartSessionCommand = new StartSessionCommand(this);

            Sessions = new ObservableCollection<Session>();

        }

        public async void Login()
        {
            try
            {
                UserClient userClient = new UserClient();
                SessionClient<Session> sessionClient = new SessionClient<Session>();
                User loggedUser = (User)await userClient.Read(Username, Password);
                if (loggedUser != null)
                {
                    if (loggedUser.Id > 0)
                    {
                        User = loggedUser;
                        Sessions = new ObservableCollection<Session>(await sessionClient.ReadByUser(loggedUser.Id));
                        RunningSession = Sessions.Where(x => x.StartDateTime < DateTime.Now && x.EndDateTime < DateTime.Now).FirstOrDefault();
                        Authenticated?.Invoke(this, new EventArgs());
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        public async void GetSessions()
        {
            SessionClient<Session> sessionClient = new SessionClient<Session>();
            if (User.Id > 0)
            {
                Sessions.Clear();
                foreach (Session session in (await sessionClient.ReadByUser(User.Id)).Where(x => x.StartDateTime > DateTime.Now))
                {
                    Sessions.Add(session);
                }
            }
        }

        public void StartSession(Session? session)
        {
            session.StartDateTime = DateTime.Now.AddSeconds(30);
            SessionClient<Session> sessionClient = new SessionClient<Session>();
            sessionClient.Update(session);
            RunningSession = session;
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


    }
}
