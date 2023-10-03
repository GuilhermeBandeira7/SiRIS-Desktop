using System;
using System.Windows.Input;

namespace SiRISApp.ViewModel.SiRIS.SessionManagement.SessionConfiguration
{
    public class UpdateSessionUsersCommand : ICommand
    {
        public SessionConfigurationViewModel vm { get; set; }

        public event EventHandler? CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public UpdateSessionUsersCommand(SessionConfigurationViewModel vm)
        {
            this.vm = vm;
        }


        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            vm.UpdateUsers();
        }
    }
}
