using System;
using System.Windows.Input;

namespace SiRISApp.ViewModel.SessionManagement
{
    public class UpdateSessionUsersCommand : ICommand
    {
        public SessionViewModel vm { get; set; }

        public event EventHandler? CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public UpdateSessionUsersCommand(SessionViewModel vm)
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
