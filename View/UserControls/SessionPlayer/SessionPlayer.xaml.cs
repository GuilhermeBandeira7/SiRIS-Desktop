using SiRISApp.Services;
using SiRISApp.View.Windows.SiRIS;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Vlc.DotNet.Wpf;

namespace SiRISApp.View.UserControls.SessionPlayer
{
    /// <summary>
    /// Interaction logic for SessionPlayer.xaml
    /// </summary>
    public partial class SessionPlayer : UserControl
    {
        public SessionPlayer()
        {
            InitializeComponent();
        }

        private void FolderButton_Click(object sender, RoutedEventArgs e)
        {
            FtpService.Instance.ShowFileManagement();
        }

        private void ShowComputerVolume_Click(object sender, RoutedEventArgs e)
        {
            ComputedAudioPopup.IsOpen = !ComputedAudioPopup.IsOpen;
        }

        private void ShowMicrophoneVolume_Click(object sender, RoutedEventArgs e)
        {
            MicrophoneAudioPopup.IsOpen = !MicrophoneAudioPopup.IsOpen;
        }

        private void UserControl_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
         /*   if((bool)e.NewValue)
            {
                if(applicationMenu == null)
                {
                    applicationMenu = new();
                    applicationMenu.Show();
                }
            }
            else
            {
                if(applicationMenu != null)
                {
                    applicationMenu.Close();
                    applicationMenu = null;
                }
            }*/
        }
    }
}
