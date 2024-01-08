using EntityMtwServer.Entities;
using SiRISApp.Services;
using SiRISApp.ViewModel.SiRIS.SessionPlayer;
using SiRISApp.ViewModel.SiRIS.SessionPlayer.SessionMembers.Cell;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;

namespace SiRISApp.ViewModel.SessionPlayer
{
    public class CellViewModel : INotifyPropertyChanged
    {
        #region CELL_PROPERTIES
        private long id;
        public long Id
        {
            get { return id; }
            set
            {
                if (id != value)
                {
                    id = value;
                    OnPropertyChanged(nameof(Id));
                }
            }
        }

        private string name = string.Empty;
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                if (name != value)
                {
                    name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }

        private string url = string.Empty;
        public string Url
        {
            get { return url; }
            set
            {
                if (url != value)
                {
                    url = value;
                    OnPropertyChanged(nameof(Url));
                }
            }
        }

        private bool requisition = false;
        public bool Requisition
        {
            get
            {
                return requisition;
            }
            set
            {
                if (requisition != value)
                {
                    requisition = value;
                    OnPropertyChanged(nameof(Requisition));
                    if (value)
                        RequisitionColor = "Red";
                    else
                        RequisitionColor = "White";
                }
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
                LedOnVisibility = value ? Visibility.Visible : Visibility.Collapsed;
                LedOffVisibility = value ? Visibility.Collapsed : Visibility.Visible;

            }
        }

        private DVC dvc = new();
        public DVC? Dvc
        {
            get { return dvc; }
            set
            {
                if (value != null && dvc.Id != value.Id)
                {
                    dvc = value;
                    OnPropertyChanged(nameof(Dvc));
                }


            }
        }

        #endregion

        #region RENDERING_PROPERTIES

        private string playerName = string.Empty;
        public string PlayerName
        {
            get { return playerName; }
            set { playerName = value; }
        }

        public bool screenPlayer = false;
        private bool ScreenPlayer
        {
            get { return screenPlayer; }
            set
            {
                screenPlayer = value;
                OnPropertyChanged(nameof(ScreenPlayer));
                Url = value ? GetScreenUrl() : GetDeviceUrl();
            }

        }

        private string requisitionColor = "White";
        public string RequisitionColor
        {
            get { return requisitionColor; }
            set
            {
                requisitionColor = value;
                OnPropertyChanged(nameof(RequisitionColor));

            }
        }

        private Visibility ledOnVisibility = Visibility.Collapsed;
        public Visibility LedOnVisibility
        {
            get { return ledOnVisibility; }
            set
            {
                ledOnVisibility = value;
                OnPropertyChanged(nameof(LedOnVisibility));
            }
        }

        private Visibility ledOffVisibility = Visibility.Collapsed;
        public Visibility LedOffVisibility
        {
            get
            {
                return ledOffVisibility;
            }
            set
            {
                ledOffVisibility = value;
                OnPropertyChanged(nameof(LedOffVisibility));
            }
        }

        #endregion

        public double timeout = 30; //seconds

        private List<User> allUsers = new();
        public ObservableCollection<CellMemberViewModel> UsersInsideSession { get; set; } = new();
        public ObservableCollection<CellMemberViewModel> UsersOutsideSession { get; set; } = new();

        public CellScreenStreamingCommand CellScreenStreamingCommand { get; set; }
        public CellDeviceStreamingCommand CellDeviceStreamingCommand { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;



        public CellViewModel()
        {
            CellScreenStreamingCommand = new(this);
            CellDeviceStreamingCommand = new(this);
        }

        public CellViewModel(EntityMtwServer.Entities.Cell cell, Session session) : this() 
        {
            Id = cell.Id;
            Name = cell.Name;
            Dvc = cell.Dvc;
            Status = Dvc != null && (DateTime.Now - Dvc.StatusDateTime).TotalSeconds <= timeout;
            Requisition = cell.Requisition;
            PlayerName = $"player_{cell.Dvc?.Id}";
            if (cell.Members != null && session.Recipients != null)
            {
                foreach (User user in cell.Members)
                    allUsers.Add(user);

                LoadInsideOutsideUsers(session);
            }
        }


        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void CheckCell(CellViewModel newCellViewModel, Session session)
        {
            Name = newCellViewModel.Name;
            Dvc = newCellViewModel.Dvc;
            Requisition = newCellViewModel.Requisition;
            PlayerName = $"player_{newCellViewModel.Dvc?.Id}";
            Status = Dvc != null && (DateTime.Now - Dvc.StatusDateTime).TotalSeconds <= timeout;


            List<long> added = newCellViewModel.allUsers.Select(s => s.Id).Except(allUsers.Select(s => s.Id)).ToList();
            List<long> removed = allUsers.Select(s => s.Id).Except(newCellViewModel.allUsers.Select(s => s.Id)).ToList();

            foreach (User user in newCellViewModel.allUsers.Where(u => added.Contains(u.Id)).ToList())
                allUsers.Add(user);

            foreach (long id in removed)
                allUsers.RemoveAt(allUsers.IndexOf(allUsers.Where(c => c.Id == id).First()));

            LoadInsideOutsideUsers(session);
        }

        public void LoadInsideOutsideUsers(Session session)
        {
            if (session.Recipients != null)
            {
                List<long> usersInside = session.Recipients.Select(r => r.Id).Intersect(allUsers.Select(au => au.Id)).ToList();
                List<long> usersOutside = allUsers.Select(au => au.Id).Except(session.Recipients.Select(r => r.Id)).ToList();

                LoadInside(usersInside);
                LoadOutside(usersOutside);
            }
        }

        private string GetScreenUrl()
        {
            ServerConfig serverConfig = ServerConfigService.Instance.GetServerConfig();
            url = $"rtmp://{serverConfig.Ip}:1935/screen_{Id}";
            return url;
        }

        private string GetDeviceUrl()
        {
            ServerConfig serverConfig = ServerConfigService.Instance.GetServerConfig();
            url = $"rtmp://{serverConfig.Ip}:1935/device_{Id}";
            return url;
        }

        private void LoadInside(List<long> usersInside)
        {
            List<long> added = usersInside.Except(UsersInsideSession.Select(s => s.Id)).ToList();
            List<long> removed = UsersInsideSession.Select(s => s.Id).Except(usersInside).ToList();

            foreach (User user in allUsers.Where(u => added.Contains(u.Id)).ToList())
            {
                System.Windows.Application.Current.Dispatcher.Invoke(() =>
                {
                    UsersInsideSession.Add(new CellMemberViewModel(user));
                });
            }

            foreach (long id in removed)
            {
                System.Windows.Application.Current.Dispatcher.Invoke(() =>
                {
                    UsersInsideSession.RemoveAt(UsersInsideSession.IndexOf(UsersInsideSession.Where(c => c.Id == id).First()));
                });
            }
        }

        private void LoadOutside(List<long> usersOutside)
        {
            List<long> added = usersOutside.Except(UsersOutsideSession.Select(s => s.Id)).ToList();
            List<long> removed = UsersOutsideSession.Select(s => s.Id).Except(usersOutside).ToList();

            foreach (User user in allUsers.Where(u => added.Contains(u.Id)).ToList())
            {
                System.Windows.Application.Current.Dispatcher.Invoke(() =>
                {
                    UsersOutsideSession.Add(new CellMemberViewModel(user));
                });
            }

            foreach (long id in removed)
            {
                System.Windows.Application.Current.Dispatcher.Invoke(() =>
                {
                    UsersOutsideSession.RemoveAt(UsersOutsideSession.IndexOf(UsersOutsideSession.Where(c => c.Id == id).First()));
                });
            }

        }


    }
}
