using EntityMtwServer.Entities;
using System;
using System.Windows.Input;

namespace SiRISApp.ViewModel.Commands
{
    public class LoginCommand : ICommand
    {
        public LoginVM ViewModel { get; set; }

        public event EventHandler? CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public LoginCommand(LoginVM vm)
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
