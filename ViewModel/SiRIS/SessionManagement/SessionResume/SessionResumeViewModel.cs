using EntityMtwServer.Entities;
using EntityMtwServer.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Office.Interop.Word;
using SiRISApp.Services;
using SiRISApp.View.Windows.SiRIS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SiRISApp.ViewModel.SiRIS.SessionManagement.SessionResume
{
    public class SessionResumeViewModel : INotifyPropertyChanged
    {

        private long id;
        public long Id
        {
            get { return id; }
            set { id = value; }
        }

        private string courseName = string.Empty;
        public string CourseName
        {
            get { return courseName; }
            set
            {
                courseName = value;
                OnPropertyChanged(nameof(CourseName));

            }
        }

        private string name = string.Empty;
        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                OnPropertyChanged($"{nameof(Name)}");
            }
        }

        private string description = string.Empty;
        public string Description
        {
            get { return description; }
            set
            {
                description = value;
                OnPropertyChanged($"{nameof(Description)}");
            }
        }

        private string color = string.Empty;
        public string Color
        {
            get { return color; }
            set
            {
                color = value;
                OnPropertyChanged(nameof(Color));
            }
        }

        private bool enable;
        public bool Enable
        {
            get { return enable; }
            set
            {
                enable = value;
                OnPropertyChanged(nameof(Enable));
                RestoreVisibility = value ? Visibility.Collapsed : Visibility.Visible;
                DeleteVisibility = !value ? Visibility.Collapsed : Visibility.Visible;
            }
        }

        private Visibility deleteVisibility;
        public Visibility DeleteVisibility
        {
            get { return deleteVisibility; }
            set
            {
                deleteVisibility = value;
                OnPropertyChanged(nameof(DeleteVisibility));

            }
        }

        private Visibility restoreVisibility;
        public Visibility RestoreVisibility
        {
            get { return restoreVisibility; }
            set
            {
                restoreVisibility = value;
                OnPropertyChanged(nameof(RestoreVisibility));

            }
        }

        private DateTime startDate = DateTime.Now;
        public DateTime StartDate
        {
            get { return startDate; }
            set
            {
                startDate = value;
                OnPropertyChanged($"{nameof(StartDate)}");
            }
        }

        private DateTime endDate = DateTime.Now.AddHours(1);
        public DateTime EndDate
        {
            get { return endDate; }
            set
            {
                endDate = value;
                OnPropertyChanged($"{nameof(EndDate)}");
            }
        }

        private DateTime startTime = DateTime.Now;
        public DateTime StartTime
        {
            get { return startTime; }
            set
            {
                startTime = value;
                OnPropertyChanged($"{nameof(StartTime)}");
            }
        }

        private DateTime endTime = DateTime.Now.AddHours(1);
        public DateTime EndTime
        {
            get { return endTime; }
            set
            {
                endTime = value;
                OnPropertyChanged($"{nameof(EndTime)}");
            }
        }

        private Course course = new();
        public Course Course
        {
            get
            {
                return course;
            }
            set
            {
                course = value;
                OnPropertyChanged($"{nameof(Course)}");
            }
        }

        public StartSessionCommand StartSessionCommand { get; set; }
        public EditSessionCommand EditSessionCommand { get; set; }
        public DeleteSessionCommand DeleteSessionCommand { get; set; }
        public RestoreSessionCommand RestoreSessionCommand { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;
        public event EventHandler? SelectSession;
        public event EventHandler? ReloadSessions;

        public SessionResumeViewModel()
        {
            StartSessionCommand = new(this);
            EditSessionCommand = new(this);
            DeleteSessionCommand = new(this);
            RestoreSessionCommand = new(this);
        }


        public SessionResumeViewModel(Session session) : this()
        {

            Id = session.Id;
            Name = session.Name;
            Description = session.Description;
            StartDate = session.StartDateTime;
            StartTime = session.StartDateTime;
            EndDate = session.EndDateTime;
            EndTime = session.EndDateTime;

            if (session.Course != null)
            {
                Course = session.Course;
                Course course = AppSessionService.Instance.Context.Courses.Include(x => x.CurriculumCourse).Where(x => x.Id == session.Course.Id).First();
                Color = session.Course.Color;
                CourseName = session.Course.Name;
            }
            else
            {
                CourseName = "Sessão";
                Color = "#282725";
            }

        }

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public async void StartSession()
        {
            SessionsService sessionsService = AppSessionService.Instance.SessionService;
            Session? session = await sessionsService.GetSession(Id);
            if (session != null)
            {
                TimeSpan duration = session.EndDateTime - session.StartDateTime;
                session.StartDateTime = DateTime.Now;
                session.EndDateTime = session.StartDateTime + duration;
                Response response = await sessionsService.PutSession(session.Id, session, AppSessionService.Instance.User.Id);
                if (!response.Result)
                    MessageService.Instance.Show("success", $"Falha ao iniciar a aula, causa: {response.Message}\r\n Para mais informações consulte o manual ou o suporte técnico especializado");


                ReloadSessions?.Invoke(this, new());

            }
        }

        public void EditSession()
        {
            SelectSession?.Invoke(this, new EditSessionEventArgs() { Id = Id });
        }

        public async void RestoreSession()
        {
            SessionsService sessionsService = AppSessionService.Instance.SessionService;
            Session? session = await sessionsService.GetSession(Id);
            if (session != null)
            {
                session.Enable = true;
                Response response = await sessionsService.PutSession(session.Id, session, AppSessionService.Instance.User.Id);
                Message message = new();
                if (response.Result)
                    MessageService.Instance.Show("success", "Aula recuperada com sucesso!");
                else
                    MessageService.Instance.Show("success", $"Falha ao recuperada a aula, causa: {response.Message}\r\n Para mais informações consulte o manual ou o suporte técnico especializado");

                System.Windows.Application.Current.Dispatcher.Invoke(delegate
                {
                    message.Show();
                });
                ReloadSessions?.Invoke(this, new());

            }

        }

        public async void RemoveSession()
        {
            if (MessageService.Instance.ShowDialog("warning", "Tem certeza que deseja remover a aula?", true))
            {
                SessionsService service = AppSessionService.Instance.SessionService;
                Response response = await service.DeleteSession(id, AppSessionService.Instance.User.Id);

                if (response.Result)
                    MessageService.Instance.Show("success", "Aula removida com sucesso!");
                else
                    MessageService.Instance.Show("success", $"Falha ao remover a aula, causa: {response.Message}\r\n Para mais informações consulte o manual ou o suporte técnico especializado");


                ReloadSessions?.Invoke(this, new());
            }

        }


    }
}
