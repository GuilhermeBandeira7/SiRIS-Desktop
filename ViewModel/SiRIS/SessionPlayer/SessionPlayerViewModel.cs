using EntityMtwServer;
using EntityMtwServer.Entities;
using EntityMtwServer.Services;
using Microsoft.EntityFrameworkCore;
using SiRISApp.Services;
using SiRISApp.ViewModel.FileManagement;
using SiRISApp.ViewModel.SessionPlayer.Commands;
using SiRISApp.ViewModel.SiRIS.SessionManagement.SessionConfiguration;
using SiRISApp.ViewModel.SiRIS.SessionPlayer;
using System;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Windows;

namespace SiRISApp.ViewModel.SessionPlayer
{
    public class SessionPlayerViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private long id = -1;
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

        private bool status = false;
        public bool Status
        {
            get { return status; }
            set
            {
                status = value;
                OnPropertyChanged(nameof(Status));
            }
        }

        private string statusText = "pauseTransmission";
        public string StatusText
        {
            get { return TranslationService.Instance.Translate(statusText); }
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

        private bool liveClass = false;
        public bool LiveClass
        {
            get { return liveClass; }
            set
            {
                liveClass = value;
                OnPropertyChanged(nameof(LiveClass));
                RecordedSessionVisibility = value ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        private string enableCameraText = "enableCamera";
        public string EnableCameraText
        {
            get { return TranslationService.Instance.Translate(enableCameraText); }
            set
            {
                enableCameraText = value;
                OnPropertyChanged(nameof(enableCameraText));

            }
        }

        private float computerVolume = 0;
        public float ComputerVolume
        {
            get
            {
                return computerVolume;
            }
            set
            {
                computerVolume = value;
                OBSService.Instance.SetVolumeValue("Desktop Áudio", value);
                OnPropertyChanged(nameof(ComputerVolume));
            }
        }

        private float microphoneVolume = 0;
        public float MicrophoneVolume
        {
            get
            {
                return microphoneVolume;
            }
            set
            {
                microphoneVolume = value;
                OBSService.Instance.SetVolumeValue("Mic/Aux", value);
                OnPropertyChanged(nameof(MicrophoneVolume));
            }
        }



        private Visibility recordedSessionVisibility;
        public Visibility RecordedSessionVisibility
        {
            get { return recordedSessionVisibility; }
            set
            {
                recordedSessionVisibility = value;
                OnPropertyChanged(nameof(RecordedSessionVisibility));

            }
        }

        private Session? activeSession = null;

        public SessionPreviewViewModel SessionPreviewViewModel { get; set; }
        public SessionMembersViewModel SessionMembersViewModel { get; set; }

        public SessionConfigurationViewModel SessionViewModel { get; set; } = new();
        public TogglePauseCommand TogglePauseCommand { get; set; }
        public StopSessionCommand StopSessionCommand { get; set; }
        public TogglePipCommand EnablePipCommand { get; set; }
        public SwitchToRecordedCommand SwitchToRecordedCommand { get; set; }

        Thread? runningSessionThread;


        public SessionPlayerViewModel()
        {
            TogglePauseCommand = new(this);
            StopSessionCommand = new(this);
            EnablePipCommand = new(this);
            SwitchToRecordedCommand = new(this);
            SessionPreviewViewModel = new();
            SessionMembersViewModel = new();
        }

        public void InitSession(long sessionId)
        {
            SessionId = sessionId;
            ComputerVolume = 50;
            MicrophoneVolume = 50;
            LoadSession();

            if (activeSession != null)
            {
                RestartSession();
                AppSessionService.Instance.RunningSessionId = sessionId;
                sessionRunning = true;
                runningSessionThread = new Thread(RunningSessionThread);
                runningSessionThread.SetApartmentState(ApartmentState.STA);
                runningSessionThread.Start();
            }
        }

        public void PauseSession()
        {
            if (activeSession != null && sessionRunning)
            {
                SessionMembersViewModel.Pause();
                activeSession.Status = false;
                Status = false;
                StatusImage = "play";
                StatusText = "restartTransmission";
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
                StatusText = "pauseTransmission";
                ComputerVolume = 50;
                MicrophoneVolume = 50;
                OBSService.Instance.StartStreaming();
                AppSessionService.Instance.Context.Entry(activeSession).State = EntityState.Modified;
                AppSessionService.Instance.Context.SaveChanges();
            }
        }

        public void StopSession()
        {
            if (activeSession != null)
            {
                sessionRunning = false;
                activeSession.Status = false;
                activeSession.EndDateTime = DateTime.Now;
                AppSessionService.Instance.Context.Entry(activeSession).State = EntityState.Modified;
                AppSessionService.Instance.Context.SaveChanges();
                AppSessionService.Instance.RunningSessionId = -1;
            }
        }

        public void SwitchToRecorded()
        {
            View.Windows.FileManagement fileManagement = new();
            fileManagement.ReturnSelectedFile += FileManagement_ReturnSelectedFile;
            fileManagement.IsSelecting = true;
            fileManagement.Show();
        }


        private async void FileManagement_ReturnSelectedFile(object? sender, EventArgs e)
        {
            ReturnSelectedFileEventArg args = (ReturnSelectedFileEventArg)e;
            string files = string.Join(";", args.SelectedFiles.ToArray());
            if (files != string.Empty && activeSession != null)
            {
                activeSession.RecordPath = files;
                activeSession.Live = false;
                await AppSessionService.Instance.SessionService.PutSession(activeSession.Id, activeSession, AppSessionService.Instance.User.Id);
                LiveClass = false;
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
                LiveClass = session.Live;
                if (session.Status)
                    StatusImage = "pause";
                else
                    StatusImage = "play";
            }
        }

        private void RunningSessionThread(object? obj)
        {


            while (SessionRunning)
            {
                MasterServerContext _context = new();
                UsersService _usersService = new(_context);
                AccessRulesService _accessRulesService = new(_context, _usersService);
                SessionsService _sessionsService = new(_context, _accessRulesService, _usersService);

                Session? session = _context
                   .Sessions
                   .Include(s => s.Events)
                   .Include(s => s.Recipients)
                   .Include(s => s.Course)
                   .Include(s => s.Class)
                   .Include(s => s.Transmitter)
                   .Where(s => s.Id == SessionId)
                   .FirstOrDefault();

                if (session == null)
                {
                    SessionRunning = false;
                    continue;
                }
                else
                {
                    if (session.EndDateTime <= DateTime.Now)
                    {
                        SessionRunning = false;
                        continue;
                    }
                }

                SessionMembersViewModel.CheckSession(session, _context);
                Thread.Sleep(2000);
            }
        }

    }
}
