using System;
using System.Windows;
using System.Windows.Shapes;
using System.IO;
using System.Reflection;
using SiRISApp.ViewModel;
using EntityMtwServer.Entities;
using SiRISApp.Services;
using SiRISApp.View.Windows.SiRIS;
using Microsoft.Extensions.Hosting.Internal;

namespace SiRISApp.View
{
    /// <summary>
    /// Lógica interna para SiRIS.xaml
    /// </summary>
    public partial class SiRIS : Window
    {
        public SiRIS()
        {
            InitializeComponent();
        }

        ApplicationsMenu? applicationMenu = null;

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
            if (SessionPlayer.applicationMenu != null)
            {
                SessionPlayer.applicationMenu.Close();
                SessionPlayer.applicationMenu = null;
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            if (Resources["vm"] != null)
            {
                SiRISViewModel viewModel = (SiRISViewModel)Resources["vm"];
                OBSService.Instance.Stop();
                if (viewModel.SessionPlayerViewModel.Status)
                    viewModel.SessionPlayerViewModel.PauseSession();
            }

            Close();
        }

        private void Window_Activated(object sender, EventArgs e)
        {
           /* if (applicationMenu != null)
            {
                applicationMenu.Close();
                applicationMenu = null;
            }*/

        }



        private void Window_Deactivated(object sender, EventArgs e)
        {
            /* if(applicationMenu == null)
             {
                 applicationMenu = new ApplicationsMenu();
                 applicationMenu.Show();
             }*/
        }

        private void Window_StateChanged(object sender, EventArgs e)
        {
            if (Resources["vm"] != null)
            {
                SiRISViewModel viewModel = (SiRISViewModel)Resources["vm"];
                if (viewModel.SessionPlayerViewModel.SessionRunning)
                {
                    if(WindowState == WindowState.Minimized)
                    {
                        if (applicationMenu != null)
                        {
                            applicationMenu.Close();
                            applicationMenu = null;
                        }
                    }
                    else if(WindowState == WindowState.Maximized)
                    {
                        if(applicationMenu == null)
                        {
                            applicationMenu = new();
                            applicationMenu.Show();
                        }
                    }
                }
            }
        }
    }
}
