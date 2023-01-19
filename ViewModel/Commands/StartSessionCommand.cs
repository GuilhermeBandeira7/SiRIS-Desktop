using EntityMtwServer.Entities;
using System;
using System.Windows.Input;

namespace SiRISApp.ViewModel.Commands
{
    public class StartSessionCommand : ICommand
    {
        public SiRISVm ViewModel { get; set; }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public StartSessionCommand(SiRISVm vm)
        {
            ViewModel = vm;
        }

        public bool CanExecute(object parameter)
        {
            if (ViewModel.RunningSession != null)
                return ViewModel.RunningSession.Id != 0;

            return true;
        }

        public void Execute(object parameter)
        {
            Session session = parameter as Session;
            ViewModel.StartSession(session);
        }
    }
}
