using EntityMtwServer.Entities;
using EntityMtwServer.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Office.Interop.Word;
using SiRISApp.Services;
using SiRISApp.View.UserControls.SessionManagement;
using SiRISApp.View.Windows.SiRIS;
using SiRISApp.ViewModel.Login;
using SiRISApp.ViewModel.SessionCalendar;
using SiRISApp.ViewModel.SessionManagement;
using SiRISApp.ViewModel.SessionPlayer;
using SiRISApp.ViewModel.SiRIS.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Windows;

namespace SiRISApp.ViewModel
{
    public class SiRISViewModel : INotifyPropertyChanged
    {

        public enum SIRIS_INDEX
        {
            LOGIN = 0,
            SESSION_MANAGER = 1,
            SESSION_CALENDAR = 2,
            SESSION_PLAYER = 3,
        }


        private int selectedIndex;
        public int SelectedIndex
        {
            get
            {
                return selectedIndex;
            }
            set
            {
                selectedIndex = value;
                OnPropertyChanged(nameof(SelectedIndex));
            }
        }


        public event PropertyChangedEventHandler? PropertyChanged;

        public LoginViewModel LoginViewModel { get; set; }
        public SessionManagementViewModel SessionManagementViewModel { get; set; }
        public SessionCalendarViewModel SessionCalendarViewModel { get; set; }
        public SessionPlayerViewModel SessionPlayerViewModel { get; set; }


        public SiRISViewModel()
        {

            LoginViewModel = new();
            SessionManagementViewModel = new();
            SessionCalendarViewModel = new();
            SessionPlayerViewModel = new();

            SelectedIndex = 0;
            LoginViewModel.Authenticated += Authenticated;

        }

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void LogOff(object? parameter)
        {
            /*user.Active = false;
            _masterServerContext.Entry(user).State = EntityState.Modified;
            _masterServerContext.SaveChanges();
            if(parameter != null)
            {
                bool exit = Convert.ToBoolean(parameter.ToString());
                if (exit)
                    Process.GetCurrentProcess().Kill();
            }*/

        }


        public async void StartSession()
        {
            if (SessionManagementViewModel.SelectedSession != null)
            {
                EntityMtwServer.Entities.Session session = await AppSessionService.Instance.Context.Sessions
                    .Include(s => s.Transmitter)
                    .Include(s => s.Course)
                    .Include(s => s.Recipients)
                    .Where(s => s.Id == SessionManagementViewModel.SelectedSession.Id).FirstAsync();
                SessionsService sessionsService = new(AppSessionService.Instance.Context);
                TimeSpan time = session.EndDateTime - session.StartDateTime;
                SessionManagementViewModel.SelectedSession.StartDate = DateTime.Now;
                SessionManagementViewModel.SelectedSession.EndDate = DateTime.Now.Add(time);
                Response response = await sessionsService.PutSession(session.Id, session);
                Message message = new();
                message.SetType(response.Result ? "sucess" : "error", response.Message);
                message.Show();
            }
        }

        private void Authenticated(object? sender, EventArgs e)
        {
            Thread thread = new(CheckActiveSession);
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
        }

        private void CheckActiveSession()
        {
            while (true)
            {
                if (!SessionPlayerViewModel.SessionRunning)
                {

                    List<EntityMtwServer.Entities.Session> sessions = AppSessionService.Instance.Context.Sessions
                       .Include(s => s.Transmitter)
                       .Include(s => s.Course)
                       .Include(s => s.Recipients)
                       .ToList();

                    sessions = sessions.Where(s => s.StartDateTime < DateTime.Now && s.EndDateTime > DateTime.Now)
                        .OrderBy(s => s.StartDateTime)
                        .ToList();
                    
                    foreach (EntityMtwServer.Entities.Session s in sessions)
                    {
                        if (s.Transmitter != null && s.Transmitter.Id == AppSessionService.Instance.User.Id)
                        {
                            System.Windows.Application.Current.Dispatcher.Invoke(delegate
                            {
                                Message message = new();
                                message.SetType("warning", "classIsAboutToStart");
                                message.ShowDialog();
                                SessionPlayerViewModel.InitSession(s.Id);
                            });
                   
                            SelectedIndex = (int)SIRIS_INDEX.SESSION_PLAYER; 
                            break;
                        }
                    }
              

                }

                Thread.Sleep(1000);
            }
        }
    }
}
