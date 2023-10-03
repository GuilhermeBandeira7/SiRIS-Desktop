using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SiRISApp.ViewModel.SiRIS.SessionManagement
{
    public class ShowRecoveredCommand : ICommand 
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

        public ShowRecoveredCommand(SessionManagementViewModel? sessionManagementViewModel)
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
            {
                SessionManagementViewModel.ShowDeleted = !SessionManagementViewModel.ShowDeleted;
                SessionManagementViewModel.ReloadSessions();
            }
        }
    }
}
