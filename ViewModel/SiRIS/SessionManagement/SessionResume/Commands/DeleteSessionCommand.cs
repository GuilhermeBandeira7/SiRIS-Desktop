using System;
using System.Windows.Input;

namespace SiRISApp.ViewModel.SiRIS.SessionManagement.SessionResume
{
    public class DeleteSessionCommand : ICommand
    {
        public SessionResumeViewModel ViewModel { get; set; }
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

        public DeleteSessionCommand(SessionResumeViewModel viewModel)
        {
            ViewModel = viewModel;
        }

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            ViewModel.RemoveSession();
        }
    }
}
