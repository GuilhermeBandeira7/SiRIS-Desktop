using System;
using System.Windows.Input;

namespace SiRISApp.ViewModel.SiRIS.SessionManagement
{
    public class ReloadSessionsCommand : ICommand
    {
        public SessionManagementViewModel? SessionManagementViewModel { get; set; }

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

        public ReloadSessionsCommand(SessionManagementViewModel? sessionManagementViewModel)
        {
            SessionManagementViewModel = sessionManagementViewModel;
        }

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            if (SessionManagementViewModel != null)
                SessionManagementViewModel.ReloadSessions();
        }
    }
}
