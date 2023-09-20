using SiRISApp.ViewModel.Login;
using System;
using System.Windows.Input;

namespace SiRISApp.ViewModel.Commands
{
    public class LoginCommand : ICommand
    {
        public LoginFormViewModel ViewModel { get; set; }

        public event EventHandler? CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public LoginCommand(LoginFormViewModel vm)
        {
            ViewModel = vm;
        }

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            ViewModel.Login();
        }
    }
}
