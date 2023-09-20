using SiRISApp.View.Windows.SiRIS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SiRISApp.ViewModel.Login
{
    public class ValidateLoginCommand : ICommand
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

        public ValidateLoginCommand(LoginConfigViewModel loginConfigViewModel)
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
            {
                loginConfigViewModel.SelectedIndex++;
            }
            else
            {
                Message message = new Message();
                message.SetType("error", "failToLogin");
                message.Show();
            }


        }
    }
}
