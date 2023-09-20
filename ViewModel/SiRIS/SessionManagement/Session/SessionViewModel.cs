using EntityMtwServer.Entities;
using EntityMtwServer.Services;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using SiRISApp.Services;
using SixLabors.ImageSharp;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Windows;
using SiRISApp.ViewModel.SiRIS;
using SiRISApp.View.Windows.SiRIS;

namespace SiRISApp.ViewModel.SessionManagement
{
    public class SessionViewModel : INotifyPropertyChanged
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

        private Visibility courseVisibility = Visibility.Hidden;
        public Visibility CourseVisibility
        {
            get { return courseVisibility; }
            set
            {
                courseVisibility = value;
                OnPropertyChanged(nameof(CourseVisibility));

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
        public ObservableCollection<User> Users { get; set; } = new();

        //TODO: CREATE GENERIC RELATION TABLE
        public ObservableCollection<UserViewModel> AvailableUsers { get; set; } = new();
        public ObservableCollection<UserViewModel> FilteredAvailableUsers { get; set; } = new();
        public ObservableCollection<UserViewModel> InsertedUsers { get; set; } = new();
        public ObservableCollection<UserViewModel> FilteredInsertedUsers { get; set; } = new();

        public event EventHandler? ReloadSessions;
        public event EventHandler? SelectSession;
        public event PropertyChangedEventHandler? PropertyChanged;

        public UpdateSessionUsersCommand UpdateUsersCommand { get; set; }
        public EditSessionCommand CreateSessionCommand { get; set; }
        public SelectSessionCommand SelectSessionCommand { get; set; }

        public SessionViewModel()
        {
            UpdateUsersCommand = new(this);
            CreateSessionCommand = new(this);
            SelectSessionCommand = new(this);

            AvailableFilter = string.Empty;
            InsertedFilter = string.Empty;
        }


        public SessionViewModel(Session session)
        {
            foreach (var course in AppSessionService.Instance.Context.CurriculumCourses.ToList())
                CurriculumCourses.Add(course);

            foreach (var user in AppSessionService.Instance.Context.Users.ToList())
                if (user.Cell != null && user.Id > 0)
                    Users.Add(user);

            Id = session.Id;
            Name = session.Name;
            Description = session.Description;
            StartDate = session.StartDateTime;
            StartTime = session.StartDateTime;
            EndDate = session.EndDateTime;
            EndTime = session.EndDateTime;

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

            UpdateUsersCommand = new(this);
            CreateSessionCommand = new(this);
            SelectSessionCommand = new(this);
        }

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void OnSelectSession()
        {
            SelectSession?.Invoke(this, EventArgs.Empty);
        }

        public async void EditSession()
        {
            Session session = new Session();
            session.Name = Name;
            session.Description = Description;
            session.StartDateTime = startDate.Date + StartTime.TimeOfDay;
            session.EndDateTime = endDate.Date + EndTime.TimeOfDay;
            session.Course = selectedCourse;
            session.Transmitter = AppSessionService.Instance.User;
            session.Recipients = Users.Where(u => InsertedUsers.Select(u => u.Id).Contains(u.Id)).ToList();
            session.Color = session.Course.Color;
            SessionsService sessionsService = new SessionsService(AppSessionService.Instance.Context);
            Response response = new();

            if (Id <= 0)
            {
                response = await sessionsService.PostSession(session);
                ReloadSessions?.Invoke(this, EventArgs.Empty);
                Message message = new();
                if(response.Result)
                {
                    string messageText = "Aula criada com sucesso!";
                    message.SetType("success", messageText);

                }
                else
                {
                    string messageText = $"Falha ao criar a aula, causa: {response.Message}\r\n Para mais informações consulte o manual ou o suporte técnico especializado";
                    message.SetType("error", messageText);

                }

                message.Show();
            }
            else
            {
                session.Id = Id;
                await sessionsService.PutSessionStudents(id, session);
                response = await sessionsService.PutSession(Id, session);

                Message message = new();
                if (response.Result)
                {
                    string messageText = "Aula editada com sucesso!";
                    message.SetType("success", messageText);

                }
                else
                {
                    string messageText = $"Falha ao criar a aula, causa: {response.Message}\r\n Para mais informações consulte o manual ou o suporte técnico especializado";
                    message.SetType("error", messageText);

                }

                message.Show();
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
