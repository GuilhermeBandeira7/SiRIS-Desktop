﻿using EntityMtwServer;
using EntityMtwServer.Entities;
using EntityMtwServer.Services;
using Microsoft.EntityFrameworkCore;
using SiRISApp.Services;
using SiRISApp.View.Windows.SiRIS;
using SiRISApp.ViewModel.Login;
using SiRISApp.ViewModel.SessionCalendar;
using SiRISApp.ViewModel.SessionPlayer;
using SiRISApp.ViewModel.SiRIS.SessionManagement;
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

        public bool run = true;

        private bool logged = false;


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



        private void Authenticated(object? sender, EventArgs e)
        {
            SelectedIndex = (int)SIRIS_INDEX.SESSION_MANAGER;
            logged = true;


            Thread thread = new(CheckActiveSession);
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();

        }

        private async void CheckActiveSession()
        {
            Thread.Sleep(3000);

            while (run)
            {
                try
                {
                    if (!SessionPlayerViewModel.SessionRunning)
                    {
                        MasterServerContext _context = new();
                        UsersService _usersService = new(_context);
                        AccessRulesService _accessRulesService = new(_context, _usersService);
                        SessionsService _sessionsService = new(_context, _accessRulesService, _usersService);

                        List<Session> sessions = await _context.Sessions
                           .Include(s => s.Transmitter)
                           .Include(s => s.Course)
                           .Include(s => s.Recipients)
                           .ToListAsync();

                        sessions = sessions.Where(s => DateTime.Now >= s.StartDateTime && DateTime.Now <= s.EndDateTime )
                            .OrderBy(s => s.StartDateTime)
                            .ToList();

                        foreach (Session s in sessions)
                        {
                            if (s.Transmitter != null && s.Transmitter.Id == AppSessionService.Instance.User.Id)
                            {
                                Application.Current.Dispatcher.Invoke(() =>
                                {
                                    MessageService.Instance.ShowDialog("warning", "classIsAboutToStart");
                                    SessionPlayerViewModel.InitSession(s.Id);
                                    SelectedIndex = (int)SIRIS_INDEX.SESSION_PLAYER;
                                });
                
                      
                                break;
                            }
                        }

                        if (!SessionPlayerViewModel.SessionRunning && SelectedIndex != (int)SIRIS_INDEX.SESSION_MANAGER)
                            SelectedIndex = (int)(SIRIS_INDEX.SESSION_MANAGER);
                    }


                }
                catch (Exception ex)
                {

                }


                Thread.Sleep(1000);
            }
        }
    }
}
