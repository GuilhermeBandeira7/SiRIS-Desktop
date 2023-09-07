using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SiRISApp.ViewModel.FileManagement.Commands
{
    public class DownloadFilesCommand : ICommand
    {
        public FileManagementViewModel ViewModel { get; set; }

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

        public DownloadFilesCommand(FileManagementViewModel viewModel)
        {
            ViewModel = viewModel;
        }

        public bool CanExecute(object? parameter)
        {
            if (ViewModel.SelectedFolder == null)
                return false;
            return ViewModel.SelectedFolder.Files.Any(x => x.IsSelected);
        }

        public void Execute(object? parameter)
        {
            ViewModel.DownloadFiles();  
        }
    }
}
