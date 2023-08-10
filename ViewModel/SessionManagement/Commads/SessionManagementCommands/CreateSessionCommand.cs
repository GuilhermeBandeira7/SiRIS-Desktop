using System;
using System.Windows.Input;

namespace SiRISApp.ViewModel.SessionManagement
{
    public class CreateSessionCommand : ICommand
    {
        SessionManagementViewModel _viewModel { get; set; }

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

        public CreateSessionCommand(SessionManagementViewModel viewModel)
        {
            _viewModel = viewModel;
        }

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            SessionViewModel sessionViewModel = new SessionViewModel();
            sessionViewModel.ReloadSessions += _viewModel.ReloadSessionEvent;
            _viewModel.SelectedSession = sessionViewModel;
        }
    }
}