using SiRISApp.Services;
using SiRISApp.View.Windows.SiRIS;
using SiRISApp.ViewModel.SiRIS.SessionManagement.SessionConfiguration;
using System;
using System.Windows;
using System.Windows.Input;

namespace SiRISApp.ViewModel.SiRIS.SessionManagement
{
    public class CreateSessionCommand : ICommand
    {
        public SessionManagementViewModel ViewModel { get; set; }

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
            ViewModel = viewModel;
        }

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            try
            {
                SessionConfigurationViewModel sessionViewModel = new SessionConfigurationViewModel();
                sessionViewModel.LoadCourses();
                sessionViewModel.ReloadSessions += ViewModel.ReloadSessionEvent;
                ViewModel.SessionConfigurationViewModel = sessionViewModel;
            }
            catch(Exception ex)
            {

                MessageService.Instance.Show("error", $"{ex.Message}");

            }
          
        }
    }
}