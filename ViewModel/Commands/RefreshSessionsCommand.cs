using EntityMtwServer.Entities;
using MTWServerApiClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SiRISApp.ViewModel.Commands
{
    public class RefreshSessionsCommand : ICommand
    {
        public SiRISVm ViewModel { get; set; }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public RefreshSessionsCommand(SiRISVm vm)
        {
            ViewModel = vm;
        }

        public bool CanExecute(object? parameter)
        {
            if (parameter == null)
                return false;
            return (parameter as User).Id > 0;
        }

        public void Execute(object? parameter)
        {
            ViewModel.GetSessions();
        }
    }
}
