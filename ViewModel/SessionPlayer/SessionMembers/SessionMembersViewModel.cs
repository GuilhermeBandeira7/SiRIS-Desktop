using EntityMtwServer.Entities;
using Microsoft.EntityFrameworkCore;
using SiRISApp.Services;
using SiRISApp.ViewModel.SessionManagement;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiRISApp.ViewModel.SessionPlayer
{
    public class SessionMembersViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public ObservableCollection<CellViewModel> Cells { get; set; } = new();

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
            throw new NotImplementedException();
        }

        public void Load(Session session)
        {
            List<Cell> cells = AppSessionService.Instance.Context.Cells
                .Include(c => c.Dvc)
                .Include(c => c.Members)
                .ToList();
            if (cells != null)
            {
                foreach (Cell cell in cells)
                {
                    if (cell.Members != null && cell.Dvc != null)
                    {
                        CellViewModel cellViewModel = new();
                        cellViewModel.Id = cell.Id;
                        cellViewModel.Name = cell.Name;
                        cellViewModel.Dvc = cell.Dvc;
                        cellViewModel.PlayerName = $"player_{cell.Dvc.Id}";
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
    }
}
