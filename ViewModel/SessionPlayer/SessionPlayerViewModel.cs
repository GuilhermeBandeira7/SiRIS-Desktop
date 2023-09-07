using EntityMtwServer.Entities;
using Microsoft.EntityFrameworkCore;
using SiRISApp.Services;
using SiRISApp.ViewModel.SessionManagement;
using SiRISApp.ViewModel.SessionPlayer.Commands;
using System;
using System.ComponentModel;
using System.Linq;

namespace SiRISApp.ViewModel.SessionPlayer
{
    public class SessionPlayerViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private long id;
        public long Id
        {
            get
            {
                return id;
            }
            set
            {
                id = value;
                OnPropertyChanged(nameof(Id));
            }
        }

        private bool status;
        public bool Status
        {
            get { return status; }
            set
            {
                status = value;
                OnPropertyChanged(nameof(Status));
            }
        }

        private string statusText = "Pausar transmisão";
        public string StatusText
        {
            get { return statusText; }
            set
            {
                statusText = value;
                OnPropertyChanged(nameof(StatusText));
            }
        }

        private string statusImage = "pause";
        public string StatusImage
        {
            get { return statusImage; }
            set
            {
                statusImage = value;
                OnPropertyChanged(nameof(StatusImage));
            }
        }

        private bool sessionRunning = false;
        public bool SessionRunning
        {
            get
            {
                return sessionRunning;
            }
            set
            {
                sessionRunning = value;
                OnPropertyChanged(nameof(SessionRunning));
            }
        }

        private long sessionId = -1;    
        public long SessionId
        {
            get
            {
                return sessionId;
            }
            set
            {
                sessionId = value;
                OnPropertyChanged(nameof(SessionId));
            }
        }
        
        private Session? activeSession = null;


        public SessionPreviewViewModel SessionPreviewViewModel { get; set; }
        public SessionMembersViewModel SessionMembersViewModel { get; set; }

        public SessionViewModel SessionViewModel { get; set; } = new();
        public TogglePauseCommand TogglePauseCommand { get; set; }
        public StopSessionCommand StopSessionCommand { get; set; }

        public SessionPlayerViewModel()
        {
            TogglePauseCommand = new(this);
            StopSessionCommand = new(this);
            SessionPreviewViewModel = new SessionPreviewViewModel();
            SessionMembersViewModel = new SessionMembersViewModel();
        }


 
        public void InitSession(long sessionId)
        {

            SessionId = sessionId;
            LoadSession();

            if (activeSession != null)
            {
                RestartSession();
                sessionRunning = true;
     

                if (activeSession.StartDateTime > DateTime.Now)
                {
                    TimeSpan duration = activeSession.EndDateTime - activeSession.StartDateTime;
                    activeSession.StartDateTime = DateTime.Now;
                    activeSession.EndDateTime = DateTime.Now + duration;
                    AppSessionService.Instance.Context.Entry(activeSession).State = EntityState.Modified;
                }

                AppSessionService.Instance.Context.SaveChanges();
            }

        }

        public void PauseSession()
        {
            if (activeSession != null && sessionRunning)
            {
                SessionMembersViewModel.Pause();
                Status = false;
                StatusImage = "play";
                StatusText = "Reiniciar transmissão";
                activeSession.Status = false;
                OBSService.Instance.PauseStreaming();
                AppSessionService.Instance.Context.Entry(activeSession).State = EntityState.Modified;
                AppSessionService.Instance.Context.SaveChanges();
            }
        }

        public void RestartSession()
        {
            if (activeSession != null)
            {
                SessionMembersViewModel.Restart();
                activeSession.Status = true;
       

                Status = true;
                StatusImage = "pause";
                StatusText = "Pausar transmissão";
                activeSession.Status = true;
                OBSService.Instance.StartStreaming();
                AppSessionService.Instance.Context.Entry(activeSession).State = EntityState.Modified;
                AppSessionService.Instance.Context.SaveChanges();
            }
        }

        public void StopSession()
        {
            if (activeSession != null)
            {
                activeSession.Status = false;
                activeSession.EndDateTime = DateTime.Now;
                AppSessionService.Instance.Context.Entry(activeSession).State = EntityState.Modified;
                AppSessionService.Instance.Context.SaveChanges();
                sessionRunning = false;
            }
        }

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void LoadSession()
        {
            Session? session = AppSessionService.Instance.Context
                .Sessions
                .Include(s => s.Events)
                .Include(s => s.Recipients)
                .Include(s => s.Course)
                .Include(s => s.Class)
                .Include(s => s.Transmitter)
                .Where(s => s.Id == SessionId)
                .FirstOrDefault();
            activeSession = session;
    
            if (session != null)
            {
                SessionViewModel = new(session);
                SessionMembersViewModel.Load(session);
                if (session.Status)
                    StatusImage = "pause";
                else
                    StatusImage = "play";
            }
        }
    }
}
