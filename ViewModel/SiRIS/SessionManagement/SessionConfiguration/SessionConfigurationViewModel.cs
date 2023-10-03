using EntityMtwServer.Entities;
using EntityMtwServer.Services;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using SiRISApp.Services;
using SixLabors.ImageSharp;
using Microsoft.EntityFrameworkCore;
using System.Windows;
using SiRISApp.View.Windows.SiRIS;
using SiRISApp.ViewModel.FileManagement;
using SiRISApp.ViewModel.SiRIS.SessionManagement.User;
using SiRISApp.ViewModel.SiRIS.SessionManagement.SessionConfiguration.Commands;

namespace SiRISApp.ViewModel.SiRIS.SessionManagement.SessionConfiguration
{
    public class SessionConfigurationViewModel : INotifyPropertyChanged
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

        private bool liveSession = true;
        public bool LiveSession
        {
            get { return liveSession; }
            set
            {
                liveSession = value;
                OnPropertyChanged(nameof(LiveSession));
                RecordInfoVisibility = !value ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        private string recordPath = string.Empty;
        public string RecordPath
        {
            get { return recordPath; }
            set
            {
                recordPath = value;
                OnPropertyChanged(nameof(RecordPath));
                Files.Clear();
                foreach (var file in recordPath.Split(";"))
                    Files.Add(file);
            }
        }

        private Course selectedCourse = new();
        public Course SelectedCourse
        {
            get
            {
                return selectedCourse;
            }
            set
            {
                selectedCourse = value;
                OnPropertyChanged($"{nameof(SelectedCourse)}");
            }
        }

        private CurriculumCourse selectedCurriculumCourse = new();
        public CurriculumCourse SelectedCurriculumCourse
        {
            get { return selectedCurriculumCourse; }
            set
            {
                selectedCurriculumCourse = value;
                Courses.Clear();
                CourseVisibility = Visibility.Collapsed;

                if (value != null)
                {
                    foreach (var course in AppSessionService.Instance.Context.Courses.ToList())
                        if (course.CurriculumCourse != null && course.CurriculumCourse.Id == value.Id)
                            Courses.Add(course);

                    CourseVisibility = Visibility.Visible;
                }
                OnPropertyChanged(nameof(SelectedCurriculumCourse));
            }
        }

        private Visibility courseVisibility = Visibility.Collapsed;
        public Visibility CourseVisibility
        {
            get { return courseVisibility; }
            set
            {
                courseVisibility = value;
                OnPropertyChanged(nameof(CourseVisibility));

            }
        }

        private Visibility recordInfoVisibility = Visibility.Collapsed;
        public Visibility RecordInfoVisibility
        {
            get { return recordInfoVisibility; }
            set
            {
                recordInfoVisibility = value;
                OnPropertyChanged(nameof(RecordInfoVisibility));
            }
        }

        private string availableFilter = string.Empty;
        public string AvailableFilter
        {
            get { return availableFilter; }
            set
            {
                availableFilter = value;
                OnPropertyChanged(nameof(AvailableFilter));
                var filteredElements = AvailableUsers.Where(x => x.Nome.Contains(value) || x.Matricula.Contains(value) || x.Id.ToString().Contains(value)).ToList();
                FilteredAvailableUsers.Clear();
                foreach (var item in filteredElements)
                    FilteredAvailableUsers.Add(item);
            }
        }

        private string insertedFilter = string.Empty;
        public string InsertedFilter
        {
            get { return insertedFilter; }
            set
            {
                insertedFilter = value;
                OnPropertyChanged(nameof(InsertedFilter));
                var filteredElements = InsertedUsers.Where(x => x.Nome.Contains(value) || x.Matricula.Contains(value) || x.Id.ToString().Contains(value)).ToList();
                FilteredInsertedUsers.Clear();
                foreach (var item in filteredElements)
                    FilteredInsertedUsers.Add(item);
            }
        }

        public ObservableCollection<CurriculumCourse> CurriculumCourses { get; set; } = new();
        public ObservableCollection<Course> Courses { get; set; } = new();
        public ObservableCollection<EntityMtwServer.Entities.User> Users { get; set; } = new();
        public ObservableCollection<string> Files { get; set; } = new();

        //TODO: CREATE GENERIC RELATION TABLE
        public ObservableCollection<UserViewModel> AvailableUsers { get; set; } = new();
        public ObservableCollection<UserViewModel> FilteredAvailableUsers { get; set; } = new();
        public ObservableCollection<UserViewModel> InsertedUsers { get; set; } = new();
        public ObservableCollection<UserViewModel> FilteredInsertedUsers { get; set; } = new();

        public event PropertyChangedEventHandler? PropertyChanged;
        public event EventHandler? ReloadSessions;

        public GetRecordFileCommand GetRecordFileCommand { get; set; }
        public UpdateSessionUsersCommand UpdateUsersCommand { get; set; }
        public SaveSessionCommand SaveSessionCommand { get; set; }
     

        public SessionConfigurationViewModel()
        {
            UpdateUsersCommand = new(this);
            SaveSessionCommand = new(this);
            GetRecordFileCommand = new(this);

            AvailableFilter = string.Empty;
            InsertedFilter = string.Empty;
        }

        public SessionConfigurationViewModel(Session session) : this()
        {
            foreach (var course in AppSessionService.Instance.Context.CurriculumCourses.ToList())
                CurriculumCourses.Add(course);

            foreach (var user in AppSessionService.Instance.Context.Users.Include(u => u.Cell).ToList())
                if (user.Cell != null && user.Id > 0)
                    Users.Add(user);

            Id = session.Id;
            Name = session.Name;
            Description = session.Description;
            StartDate = session.StartDateTime;
            StartTime = session.StartDateTime;
            EndDate = session.EndDateTime;
            EndTime = session.EndDateTime;
            LiveSession = session.Live;
            RecordPath = session.RecordPath;


            if (session.Course != null)
            {
                Course course = AppSessionService.Instance.Context.Courses.Include(x => x.CurriculumCourse).Where(x => x.Id == session.Course.Id).First();
                if (course.CurriculumCourse != null)
                    SelectedCurriculumCourse = course.CurriculumCourse;
                SelectedCourse = session.Course;
                Color = session.Course.Color;
                CourseName = session.Course.Name;
            }
            else
            {
                CourseName = "Sessão";
                Color = "#282725";
            }

            if (session.Recipients != null)
            {

                foreach (var student in session.Recipients)
                    InsertedUsers.Add(new UserViewModel(student));

                foreach (var user in Users.Where(u => !session.Recipients.Select(s => s.Id).Contains(u.Id)).ToList())
                    AvailableUsers.Add(new UserViewModel(user));

                AvailableFilter = string.Empty;
                InsertedFilter = string.Empty;
            }


        } 

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }



        public async void SaveSession()
        {
            Session session = new()
            {
                Name = Name,
                Description = Description,
                StartDateTime = startDate.Date + StartTime.TimeOfDay,
                EndDateTime = endDate.Date + EndTime.TimeOfDay,
                Course = selectedCourse,
                Transmitter = AppSessionService.Instance.User,
                Recipients = Users.Where(u => InsertedUsers.Select(u => u.Id).Contains(u.Id)).ToList(),
                Record = LiveSession,
                Color = selectedCourse.Color,
                Live = LiveSession,
                RecordPath = RecordPath,
            };

            session.Recipients = Users.Where(u => InsertedUsers.Select(iu => iu.Id).Contains(u.Id)).ToList();
            SessionsService sessionsService = AppSessionService.Instance.SessionService;
            long userId = AppSessionService.Instance.User.Id;
            Response response = new();

            if (Id <= 0)
            {
                response = await sessionsService.PostSession(session, userId);
                ReloadSessions?.Invoke(this, EventArgs.Empty);

                if (response.Result)
                    MessageService.Instance.Show("success", "Aula criada com sucesso!");
                else
                    MessageService.Instance.Show("success", $"Falha ao criar a aula, causa: {response.Message}\r\n Para mais informações consulte o manual ou o suporte técnico especializado");
            }
            else
            {
                session.Id = Id;
                await sessionsService.PutSessionStudents(id, session);
                response = await sessionsService.PutSession(Id, session, userId);

                if (response.Result)
                    MessageService.Instance.Show("success", "Aula editada com sucesso!");
                else
                    MessageService.Instance.Show("success", $"Falha ao criar a aula, causa: {response.Message}\r\n Para mais informações consulte o manual ou o suporte técnico especializado");
            }
        }

        public void LoadCourses()
        {
            foreach (var course in AppSessionService.Instance.Context.CurriculumCourses.ToList())
                CurriculumCourses.Add(course);

            foreach (var user in AppSessionService.Instance.Context.Users.Include(u => u.Cell).ToList())
            {
                if (user.Cell != null && user.Cell.Id > 0)
                {
                    Users.Add(user);
                    AvailableUsers.Add(new UserViewModel(user));
                }

            }

            AvailableFilter = string.Empty;
            InsertedFilter = string.Empty;
        }

        public void GetRecordFile()
        {
            View.Windows.FileManagement fileManagement = new();
            fileManagement.ReturnSelectedFile += FileManagement_ReturnSelectedFile;
            fileManagement.IsSelecting = true;
            fileManagement.Show();
        }

        private void FileManagement_ReturnSelectedFile(object? sender, EventArgs e)
        {
            ReturnSelectedFileEventArg args = (ReturnSelectedFileEventArg)e;
            RecordPath = string.Join(";", args.SelectedFiles.ToArray());
        }

        //TODO: CONVERT TO GENERIC CLASS
        #region RELATION_TABLE

        public void UpdateUsers()
        {
            for (int counter = 0; counter < AvailableUsers.Count; counter++)
            {
                var user = AvailableUsers.ElementAt(counter);
                if (user.Selecionado)
                {
                    user.Selecionado = false;
                    InsertedUsers.Add(user);
                    AvailableUsers.Remove(user);
                    counter--;
                }
            }

            for (int counter = 0; counter < InsertedUsers.Count; counter++)
            {
                var user = InsertedUsers.ElementAt(counter);
                if (user.Selecionado)
                {
                    user.Selecionado = false;
                    AvailableUsers.Add(user);
                    InsertedUsers.Remove(user);
                    counter--;
                }
            }

            AvailableFilter = string.Empty;
            InsertedFilter = string.Empty;
        }


        public bool SelectAvailableUser(string registration)
        {
            var user = FilteredAvailableUsers.Where(x => x.Matricula.Contains(registration)).FirstOrDefault();
            if (user != null)
                user.Selecionado = true;

            return true;
        }

        public bool SelectInsertedUser(string registration)
        {
            var user = FilteredInsertedUsers.Where(x => x.Matricula.Contains(registration)).FirstOrDefault();
            if (user != null)
                user.Selecionado = true;

            return true;
        }



        #endregion


    }
}
