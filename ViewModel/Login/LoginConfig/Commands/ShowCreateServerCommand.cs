using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SiRISApp.ViewModel.Login
{
    public class ShowCreateServerCommand : ICommand
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

        public ShowCreateServerCommand(LoginConfigViewModel loginConfigViewModel)
        {
            this.loginConfigViewModel = loginConfigViewModel;
        }

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            if (loginConfigViewModel.Username == "mtw" && loginConfigViewModel.Password == "Senha1@siris")
                loginConfigViewModel.SelectedIndex = 3;

        }
    }
}
