using EntityMtwServer.Entities;
using Microsoft.EntityFrameworkCore;
using SiRISApp.Services;
using SiRISApp.ViewModel.SessionManagement;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public ObservableCollection<CellViewModel> Cells { get; set; } = new();
        public SessionViewModel SessionViewModel { get; set; } = new();


        public void InitSession(long sessionId)
        {
            Session? session = AppSessionService.Instance.Context
                 .Sessions
                 .Include(s => s.Events)
                 .Include(s => s.Recipients)
                 .Include(s => s.Course)
                 .Include(s => s.Class)
                 .Include(s => s.Transmitter)
                 .Where(s => s.Id == sessionId)
                 .FirstOrDefault();

            if (session != null)
            {
                SessionViewModel = new(session);
                List<Cell> cells = AppSessionService.Instance.Context.Cells
                    .Include(c => c.Dvc)
                    .Include(c => c.Members)
                    .ToList();
                foreach (Cell cell in cells)
                {
                    if (cell.Members != null && cell.Dvc != null)
                    {
                        CellViewModel cellViewModel = new();
                        cellViewModel.Id = cell.Id;
                        cellViewModel.Name = cell.Name;
                        cellViewModel.Dvc = cell.Dvc;
                        foreach (User user in cell.Members)
                        {
                            if (session.Recipients != null)
                                if (session.Recipients.Select(r => r.Id).Contains(user.Id))
                                    cellViewModel.UsersInsideSession.Add(new UserViewModel(user));
                                else
                                    cellViewModel.UsesOutsideSession.Add(new UserViewModel(user));
                        }
                            
                        Cells.Add(cellViewModel);   
        
                    }
                }
            }
        }


        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
