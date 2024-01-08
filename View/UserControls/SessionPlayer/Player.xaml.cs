using EntityMtwServer.Entities;
using SiRISApp.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
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
        public bool restart = false;
        public string url = string.Empty;
        public string Url
        {
            get { return (String)GetValue(UrlProperty); }
            set { SetValue(UrlProperty, value); }
        }

        public string Volume { get; set; } = "0";

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty UrlProperty =
            DependencyProperty.Register("Url", typeof(string), typeof(Player), new PropertyMetadata(null, SetValues));

        private static void SetValues(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Player? playerControl = d as Player;
            if (playerControl != null)
            {
                if (playerControl.url != playerControl.Url)
                    playerControl.restart = true;
                playerControl.url = playerControl.Url;
            }
        }

        public bool run = false;
        Thread? thread = null;

        public Player()
        {
            InitializeComponent();
        }

        private void RestartPlayerThread(object? obj)
        {
            Thread.Sleep(10000);

            while (run)
            {
                try
                {
                    bool isPlayig = VlcControl.SourceProvider.MediaPlayer.IsPlaying();
                    if (!isPlayig || restart)
                    {
                        ServerConfig serverConfig = ServerConfigService.Instance.GetServerConfig();
                        User user = AppSessionService.Instance.User;
                        if (url == string.Empty)
                            url = $"rtmp://{serverConfig.Ip}:1935/stream_{user.Id}";
                        VlcControl.SourceProvider.MediaPlayer.Play(new Uri(url));
                        restart = false;
                    }
                }
                catch
                {

                }
               

                Thread.Sleep(10000);
            }
        }

        public void Start()
        {
            Thread.Sleep(5000);

            if (url == string.Empty)
            {
                ServerConfig serverConfig = ServerConfigService.Instance.GetServerConfig();
                User user = AppSessionService.Instance.User;
                url = $"rtmp://{serverConfig.Ip}:1935/stream_{user.Id}";

            }

            VlcControl.SourceProvider.MediaPlayer.Play(new Uri(url));
            if(thread != null && thread.IsAlive) 
                thread.Join();
            run = true;
            thread = new(RestartPlayerThread)
            {
                Name = "PLAYER THREAD"
            };
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();

        }

        private void UserControl_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Convert.ToBoolean(e.NewValue))
            {
                var currentAssembly = Assembly.GetEntryAssembly();
                if (currentAssembly != null)
                {
                    var currentDirectory = new FileInfo(currentAssembly.Location).DirectoryName;
                    if (currentDirectory != null)
                    {
                        var libDirectory = new DirectoryInfo(Path.Combine(currentDirectory, "_ExternalApps", "libvlc", IntPtr.Size == 4 ? "win-x86" : "win-x64"));
                        VlcControl.SourceProvider.CreatePlayer(libDirectory);
                        VlcControl.SourceProvider.MediaPlayer.Audio.ToggleMute();
                        Task.Run(() => Start());
                    }
                }
            }
            else
            {
                run = false;
            }
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            try
            {
                Volume = Convert.ToInt32(e.NewValue).ToString();
                if (VlcControl.SourceProvider.MediaPlayer != null)
                {
                    if (Volume == "0")
                    {
                        if (VlcControl.SourceProvider.MediaPlayer.Audio.IsMute)
                            VlcControl.SourceProvider.MediaPlayer.Audio.ToggleMute();
                    }
                    else
                    {

                        if (VlcControl.SourceProvider.MediaPlayer.Audio.IsMute)
                            VlcControl.SourceProvider.MediaPlayer.Audio.ToggleMute();

                        VlcControl.SourceProvider.MediaPlayer.Audio.Volume = int.Parse(Volume);
                    }
                }
            }
            catch
            {

            }
      
 
        }
    }
}
