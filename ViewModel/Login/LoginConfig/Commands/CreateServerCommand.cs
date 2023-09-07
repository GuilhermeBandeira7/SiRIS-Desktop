using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SiRISApp.ViewModel.Login
{
    public class CreateServerCommand : ICommand
    {
        LoginConfigViewModel loginConfigViewModel;

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

        public CreateServerCommand(LoginConfigViewModel loginConfigViewModel)
        {
            this.loginConfigViewModel = loginConfigViewModel;
        }

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            loginConfigViewModel.CreateServer();
        }
    }
}
