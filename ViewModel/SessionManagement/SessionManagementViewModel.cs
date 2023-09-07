using Microsoft.EntityFrameworkCore;
using SiRISApp.Services;
using SiRISApp.ViewModel.SessionManagement.Commads;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace SiRISApp.ViewModel.SessionManagement
{
    public class SessionManagementViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public ObservableCollection<SessionViewModel> Sessions { get; set; } = new();

        private SessionViewModel selectedSession = new();
        public SessionViewModel SelectedSession
        {
            get { return selectedSession; }
            set
            {
                if (value != null)
                {
                    selectedSession = value;
                    OnPropertyChanged(nameof(SelectedSession));
                }
            }
        }


        public ReloadSessionsCommand ReloadSessionsCommand { get; set; }
        public CreateSessionCommand CreateSessionCommand { get; set; }

        public SessionManagementViewModel()
        {
            ReloadSessionsCommand = new(this);
            CreateSessionCommand = new(this);
        }

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void ReloadSessionEvent(object? sender, EventArgs e)
        {
            ReloadSessions();
        }

        public void SelectSessionEvent(object? sender, EventArgs e)
        {
            if (sender != null)
            {
                SessionViewModel selectedSession = (SessionViewModel)sender;
                SelectedSession = selectedSession;
            }
        }

        public void ReloadSessions()
        {
            Sessions.Clear();

            List<EntityMtwServer.Entities.Session> sessions = AppSessionService.Instance.Context.Sessions
                .Include(s => s.Course)
                .Include(s => s.Recipients)
                .Include(s => s.Transmitter)
                .Where(s => s.StartDateTime > DateTime.Now.Date)
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
                    SessionViewModel sessionViewModel = new SessionViewModel(session);
                    sessionViewModel.ReloadSessions += ReloadSessionEvent;
                    sessionViewModel.SelectSession += SelectSessionEvent;
                    Sessions.Add(sessionViewModel);
                }
            }
        }
    }
}
