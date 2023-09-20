using EntityMtwServer.Entities;
using SiRISApp.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Policy;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Path = System.IO.Path;

namespace SiRISApp.View.UserControls.SessionPlayer
{
    /// <summary>
    /// Interaction logic for Player.xaml
    /// </summary>
    public partial class Player : UserControl
    {
        public Player()
        {
            InitializeComponent();
        }

        string url = string.Empty;
        public void Start()
        {
            Thread.Sleep(5000);

            ServerConfig serverConfig = ServerConfigService.Instance.GetServerConfig();
            User user = AppSessionService.Instance.User;
            url = $"rtmp://{serverConfig.Ip}:1935/stream_{user.Id}";
            this.VlcControl.SourceProvider.MediaPlayer.Play(new Uri(url));
        }

        private void UserControl_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Convert.ToBoolean(e.NewValue))
            {

                var currentAssembly = Assembly.GetEntryAssembly();
                var currentDirectory = new FileInfo(currentAssembly.Location).DirectoryName;
                var libDirectory = new DirectoryInfo(Path.Combine(currentDirectory, "_ExternalApps" ,"libvlc", IntPtr.Size == 4 ? "win-x86" : "win-x64"));

                this.VlcControl.SourceProvider.CreatePlayer(libDirectory);
                this.VlcControl.SourceProvider.MediaPlayer.Audio.ToggleMute();
                Task.Run(() => Start());
            }
        }

    }
}
