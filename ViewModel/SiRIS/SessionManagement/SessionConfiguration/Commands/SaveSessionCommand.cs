using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SiRISApp.ViewModel.SiRIS.SessionManagement.SessionConfiguration.Commands
{
    public class SaveSessionCommand : ICommand
    {
        private readonly SessionConfigurationViewModel ViewModel;

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

        public SaveSessionCommand(SessionConfigurationViewModel viewModel)
        {
            ViewModel = viewModel;
        }


        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            ViewModel.SaveSession();
        }
    }
}
