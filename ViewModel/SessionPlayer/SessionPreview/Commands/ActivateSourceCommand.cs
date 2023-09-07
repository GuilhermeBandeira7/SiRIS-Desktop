using SiRISApp.Services;
using System;
using System.Windows.Input;

namespace SiRISApp.ViewModel.SessionPlayer.Commands
{
    public class ActivateSourceCommand : ICommand
    {
        SessionPreviewViewModel viewModel;

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

        public ActivateSourceCommand(SessionPreviewViewModel viewModel)
        {
            this.viewModel = viewModel;
        }

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            OBSService.Instance.ActivateSource(viewModel.SelectedSource);
        }
    }
}
