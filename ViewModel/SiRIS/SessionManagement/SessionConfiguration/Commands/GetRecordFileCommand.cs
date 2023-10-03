using System;
using System.Windows.Input;

namespace SiRISApp.ViewModel.SiRIS.SessionManagement.SessionConfiguration
{
    public class GetRecordFileCommand : ICommand
    {
        public SessionConfigurationViewModel ViewModel { get; set; }
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

        public GetRecordFileCommand(SessionConfigurationViewModel ViewModel)
        {
            this.ViewModel = ViewModel;
        }

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            ViewModel.GetRecordFile();
        }
    }
}
