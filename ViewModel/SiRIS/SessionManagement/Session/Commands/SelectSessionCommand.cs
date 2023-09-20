using System;
using System.Windows.Input;

namespace SiRISApp.ViewModel.SessionManagement
{
    public class SelectSessionCommand : ICommand
    {

        public SessionViewModel? SessionViewModel { get; set; }

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

        public SelectSessionCommand(SessionViewModel? sessionViewModel)
        {
            if (sessionViewModel != null)
                SessionViewModel = sessionViewModel;
        }

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            SessionViewModel?.OnSelectSession();
        }
    }
}
