using EntityMtwServer.Entities;
using System;
using System.Windows.Input;

namespace SiRISApp.ViewModel.Commands
{
    public class LoginCommand : ICommand
    {
        public SiRISVm ViewModel { get; set; }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public LoginCommand(SiRISVm vm)
        {
            ViewModel = vm;
        }

        public bool CanExecute(object parameter)
        {
            User user = parameter as User;
            if (user == null)
                return false;
            return !string.IsNullOrEmpty(user.Password) && !string.IsNullOrEmpty(user.Login);
        }

        public void Execute(object parameter)
        {
            ViewModel.Login();
        }
    }
}
