using EntityMtwServer.Entities;
using EntityMtwServer.Services;
using Microsoft.EntityFrameworkCore;
using SiRISApp.Services;
using SiRISApp.View.Windows.SiRIS;
using SiRISApp.ViewModel.SiRIS.SessionManagement.SessionConfiguration;
using SiRISApp.ViewModel.SiRIS.SessionManagement.SessionResume;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace SiRISApp.ViewModel.SiRIS.SessionManagement
{
    public class SessionManagementViewModel : INotifyPropertyChanged
    {

        public enum SESSION_MANAGEMENT_INDEX
        {
            MENU = 0,
            DETAILS = 1,
            CALENDAR = 2,
        }

        private int index;
        public int Index
        {
            get { return index; }
            set
            {
                index = value;
                OnPropertyChanged(nameof(Index));

            }
        }

        private bool showDeleted = false;
        public bool ShowDeleted
        {
            get { return showDeleted; }
            set
            {
                showDeleted = value;
                OnPropertyChanged(nameof(showDeleted));

            }
        }


        public ObservableCollection<SessionResumeViewModel> Sessions { get; set; } = new();

        private SessionConfigurationViewModel sessionConfigurationViewModel = new();
        public SessionConfigurationViewModel SessionConfigurationViewModel
        {
            get { return sessionConfigurationViewModel; }
            set
            {
                if (value != null)
                {
                    sessionConfigurationViewModel = value;
                    OnPropertyChanged(nameof(SessionConfigurationViewModel));
                }
            }
        }


        public ReloadSessionsCommand ReloadSessionsCommand { get; set; }
        public CreateSessionCommand CreateSessionCommand { get; set; }
        public ShowRecoveredCommand ShowRecoveredCommand { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;

        public SessionManagementViewModel()
        {
            ReloadSessionsCommand = new(this);
            CreateSessionCommand = new(this);
            ShowRecoveredCommand = new(this);
        }

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void ReloadSessionEvent(object? sender, EventArgs e)
        {
            ReloadSessions();
        }

        public async void SelectSessionEvent(object? sender, EventArgs e)
        {
            if (sender != null)
            {
                SessionsService sessionsService = AppSessionService.Instance.SessionService;
                Session? session = await sessionsService.GetSession(((EditSessionEventArgs)e).Id);
                if (session != null)
                {
                    SessionConfigurationViewModel = new SessionConfigurationViewModel(session);
                    Index = (int)SESSION_MANAGEMENT_INDEX.DETAILS;
                }
                else
                {
                    MessageService.Instance.Show("success", "Sessao inexistente");
                }

            }
        }

        public void ReloadSessions()
        {
            Sessions.Clear();

            List<Session> sessions = AppSessionService.Instance.Context.Sessions
                .Include(s => s.Course)
                .Include(s => s.Recipients)
                .Include(s => s.Transmitter)
                .Where(s => s.StartDateTime > DateTime.Now.Date)
                .Where(s => s.Enable == !ShowDeleted)
                .AsNoTracking()
                .ToList();

            sessions = sessions.Where(s => s.StartDateTime > DateTime.Now).ToList();

            foreach (var session in sessions)
            {
                if (session.Course != null && session.Transmitter != null && session.Transmitter.Id == AppSessionService.Instance.User.Id)
                {
                    session.Course = AppSessionService.Instance.Context.Courses
                        .Include(c => c.Image)
                        .Where(c => c.Id == session.Course.Id)
                        .First();
                    SessionResumeViewModel sessionViewModel = new SessionResumeViewModel(session);
                    sessionViewModel.ReloadSessions += ReloadSessionEvent;
                    sessionViewModel.SelectSession += SelectSessionEvent;
                    sessionViewModel.Enable = session.Enable;
                    Sessions.Add(sessionViewModel);
                }
            }
        }
    }
}
