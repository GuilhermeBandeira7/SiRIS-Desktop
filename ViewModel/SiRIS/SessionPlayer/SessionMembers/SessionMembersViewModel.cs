using EntityMtwServer;
using EntityMtwServer.Entities;
using EntityMtwServer.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Office.Interop.Word;
using SiRISApp.Services;
using SiRISApp.ViewModel.SiRIS.SessionManagement.User;
using SiRISApp.ViewModel.SiRIS.SessionPlayer;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace SiRISApp.ViewModel.SessionPlayer
{
    public class SessionMembersViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;


        private string newUserText = string.Empty;
        public string NewUserText
        {
            get { return newUserText; }
            set
            {
                newUserText = value;
                OnPropertyChanged(nameof(NewUserText));

            }
        }

        private bool searchByName = false;
        public bool SearchByName
        {
            get { return searchByName; }
            set
            {
                searchByName = value;
                OnPropertyChanged(nameof(SearchByName));
                if(value)
                    SearchByRegistration = !value;
            }
        }

        private bool searchByRegistration = true;
        public bool SearchByRegistration
        {
            get { return searchByRegistration; }
            set
            {
                searchByRegistration = value;
                OnPropertyChanged(nameof(SearchByRegistration));
                if (value)
                    SearchByName = !value;
            }
        }




        private UserViewModel selectedUser = new();
        public UserViewModel SelectedUser
        {
            get { return selectedUser; }
            set
            {
                selectedUser = value;
                OnPropertyChanged(nameof(SelectedUser));
            }
        }

        public ObservableCollection<CellViewModel> Cells { get; set; } = new();


        public AddUserToSessionCommand AddUserToSessionCommand { get; set; }

        public SessionMembersViewModel()
        {
            AddUserToSessionCommand = new(this);
        }

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Pause()
        {
            foreach (CellViewModel cell in Cells)
            {
                if (cell.Dvc != null)
                {
                    cell.Dvc.Paused = true;
                    AppSessionService.Instance.Context.Entry(cell.Dvc).State = EntityState.Modified;
                }
            }

            AppSessionService.Instance.Context.SaveChanges();


        }

        public void Restart()
        {
            foreach (CellViewModel cell in Cells)
            {
                if (cell.Dvc != null)
                {
                    cell.Dvc.Paused = false;
                    AppSessionService.Instance.Context.Entry(cell.Dvc).State = EntityState.Modified;
                }
            }

            AppSessionService.Instance.Context.SaveChanges();
        }


        public void Load(Session session)
        {
            Cells.Clear();
            foreach (CellViewModel cell in GetSessionCells(session, AppSessionService.Instance.Context))
                Cells.Add(cell);
        }

        public void CheckSession(Session session, MasterServerContext context)
        {
            List<CellViewModel> newCells = GetSessionCells(session, context);

            List<long> added = newCells.Select(s => s.Id).Except(Cells.Select(s => s.Id)).ToList();
            List<long> removed = Cells.Select(s => s.Id).Except(newCells.Select(s => s.Id)).ToList();

            foreach (CellViewModel cell in newCells.Where(nc => added.Contains(nc.Id)))
                Cells.Add(cell);

            foreach (long id in removed)
                Cells.RemoveAt(Cells.IndexOf(Cells.Where(c => c.Id == id).First()));

            foreach (CellViewModel cell in Cells)
                cell.CheckCell(newCells.Where(nc => nc.Id == cell.Id).First(), session);

        }



        private List<CellViewModel> GetSessionCells(Session session, MasterServerContext context)
        {
           
            List<EntityMtwServer.Entities.Cell> cells = context.Cells
                .Include(c => c.Dvc)
                .Include(c => c.Members)
                .ToList();

            List<CellViewModel> Cells = new();
            foreach (EntityMtwServer.Entities.Cell cell in cells)
            {
                context.Entry(cell).Reload();
                Cells.Add(new(cell, session));
            }
                

            return Cells;

        }
    }
}
