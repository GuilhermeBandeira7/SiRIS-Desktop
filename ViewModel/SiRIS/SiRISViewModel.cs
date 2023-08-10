using EntityMtwServer.Entities;
using Microsoft.EntityFrameworkCore;
using SiRISApp.Services;
using SiRISApp.ViewModel.SessionManagement;
using SiRISApp.ViewModel.SessionPlayer;
using SiRISApp.ViewModel.SiRIS.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace SiRISApp.ViewModel
{
    public class SiRISViewModel : INotifyPropertyChanged
    {
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

        public SessionManagementViewModel SessionManagementViewModel { get; set; }
        public SessionPlayerViewModel SessionPlayerViewModel { get; set; }

        public StartSessionCommand StartSessionCommand { get; set; }

        public SiRISViewModel()
        {
            StartSessionCommand = new(this);
            SessionManagementViewModel = new();
            SessionPlayerViewModel = new();
            SelectedIndex = 0;

            OBSService.Instance.Start();

            List<Session> sessions = AppSessionService.Instance.Context.Sessions
                .Include(s => s.Transmitter)
                .Include(s => s.Course)
                .Include(s => s.Recipients)
                .Where(s => s.StartDateTime < DateTime.Now && s.EndDateTime > DateTime.Now)
                .ToList();

            foreach (Session s in sessions)
            {
                if (s.Transmitter != null && s.Transmitter.Id == AppSessionService.Instance.User.Id)
                {
                    SessionPlayerViewModel.InitSession(s.Id);
                    SelectedIndex = 1;
                    break;
                }
            }
         
        }

        private void OnPropertyChanged(string propertyName)
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

        public void StartSession()
        {
            if (SessionManagementViewModel.SelectedSession != null)
                SessionPlayerViewModel.InitSession(SessionManagementViewModel.SelectedSession.Id);
        }
    }
}
