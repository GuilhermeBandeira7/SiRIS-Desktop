using SiRISApp.Services;
using SiRISApp.ViewModel.SessionPlayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SiRISApp.ViewModel.SiRIS.SessionPlayer
{
    public class AddUserToSessionCommand : ICommand
    {
        SessionMembersViewModel ViewModel { get; set; }
        public event EventHandler? CanExecuteChanged
        {
            add
            {
                CommandManager.RequerySuggested += value;
            }
            remove
            {
                CommandManager.RequerySuggested -= value;
            }
        }

        public AddUserToSessionCommand(SessionMembersViewModel viewModel)
        {
            ViewModel = viewModel;
        }

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public async void Execute(object? parameter)
        {
            long? id = -1;
            if(ViewModel.SearchByName)
                id = AppSessionService.Instance.Context.Users.Where(u => u.Name == ViewModel.NewUserText).Select(u => u.Id).FirstOrDefault();
            else
                id = AppSessionService.Instance.Context.Users.Where(u => u.Registration == ViewModel.NewUserText).Select(u => u.Id).FirstOrDefault();

            if (id != null)
                await AppSessionService.Instance.SessionService.PutUserOnSession(AppSessionService.Instance.RunningSessionId, (long)id);
        }
    }
}
