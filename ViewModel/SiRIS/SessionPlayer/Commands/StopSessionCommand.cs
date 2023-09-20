using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SiRISApp.ViewModel.SessionPlayer.Commands
{
    public class StopSessionCommand : ICommand
    {
        public SessionPlayerViewModel ViewModel { get; set; }
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

        public StopSessionCommand(SessionPlayerViewModel sessionPlayerViewModel)
        {
            ViewModel = sessionPlayerViewModel;
        }

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            ViewModel.StopSession();
        }
    }
}
