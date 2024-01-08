using SiRISApp.Services;
using SiRISApp.ViewModel.SessionPlayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SiRISApp.ViewModel.SiRIS.SessionPlayer
{
    public class CellScreenStreamingCommand : ICommand
    {
        public CellViewModel ViewModel { get; set; }
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

        public CellScreenStreamingCommand(CellViewModel viewModel)
        {
            ViewModel = viewModel;
        }

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            ServerConfig serverConfig = ServerConfigService.Instance.GetServerConfig();
            string url = $"rtsp://{serverConfig.Ip}:8554/screen_{ViewModel.Dvc?.Id}";

            ViewModel.Url = url;
        }
    }
}
