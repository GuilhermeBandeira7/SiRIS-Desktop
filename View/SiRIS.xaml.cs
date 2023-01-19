using System;
using System.Windows;
using System.Windows.Shapes;
using System.IO;
using System.Reflection;

namespace SiRISApp.View
{
    /// <summary>
    /// Lógica interna para SiRIS.xaml
    /// </summary>
    public partial class SiRIS : Window
    {
        ViewModel.SiRISVm vm;

        public SiRIS(ViewModel.SiRISVm viewModel)
        {
            InitializeComponent();
            vm = Resources["vm"] as ViewModel.SiRISVm;
            vm.User = viewModel.User;
          
            var currentAssembly = Assembly.GetEntryAssembly();
            var currentDirectory = new FileInfo(currentAssembly.Location).DirectoryName;
            var libDirectory = new DirectoryInfo(System.IO.Path.Combine(currentDirectory, "libvlc", IntPtr.Size == 4 ? "win-x86" : "win-x64"));

            //this.MainPlayer.SourceProvider.CreatePlayer(libDirectory/* pass your player parameters here */);
           // this.VlcControl.SourceProvider.MediaPlayer.Play(new Uri("rtsp://172.16.2.170:8554/ms"));
        }
    }
}
