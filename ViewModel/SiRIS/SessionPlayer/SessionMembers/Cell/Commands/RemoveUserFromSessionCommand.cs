using SiRISApp.Services;
using SiRISApp.ViewModel.SessionPlayer;
using SiRISApp.ViewModel.SiRIS.SessionPlayer.SessionMembers.Cell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SiRISApp.ViewModel.SiRIS.SessionPlayer
{
    public class RemoveUserFromSessionCommand : ICommand
    {
        public CellMemberViewModel ViewModel { get; set; } 
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

        public RemoveUserFromSessionCommand(CellMemberViewModel viewModel)
        {
            ViewModel = viewModel;
        }

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public async void Execute(object? parameter)
        {
            if(parameter != null)
            {
                long? sessionId = (long)parameter;
                if (sessionId != null)
                    await AppSessionService.Instance.SessionService.DeleteUserFromSession(AppSessionService.Instance.RunningSessionId, ViewModel.Id);

            }
        }
    }
}
