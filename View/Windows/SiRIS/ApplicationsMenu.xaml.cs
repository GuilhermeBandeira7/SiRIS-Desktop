using MahApps.Metro.Controls;
using SiRISApp.ViewModel.SiRIS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SiRISApp.View.Windows.SiRIS
{
    /// <summary>
    /// Interaction logic for ApplicationsMenu.xaml
    /// </summary>
    public partial class ApplicationsMenu : Window
    {
        public delegate void ShowPanel();
        ShowPanel ShowPanelDelegate;

        public bool state = false;
        public ApplicationsMenu()
        {
            InitializeComponent();
            ShowPanelDelegate = new ShowPanel(ShowPanelCallback);
        }

        private void ShowPanelCallback()
        {
            ApplicationMenu.Visibility = Visibility.Visible;
            if (Resources["vm"] != null)
            {
                ApplicationMenuViewModel viewModel = (ApplicationMenuViewModel)Resources["vm"];
                viewModel.BackgroundColor = "transparent";
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Topmost = true;
            Top = 0;
            Left = (1920/2) - (Width /2);
        }

        private void Window_Deactivated(object sender, EventArgs e)
        {
            Activate();
            Topmost = true;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {


            if (!state)
            {
                if (Resources["vm"] != null)
                {
                    ApplicationMenuViewModel viewModel = (ApplicationMenuViewModel)Resources["vm"];
                    viewModel.BackgroundColor = "white";
                }
              
                BeginStoryboard((Storyboard)Resources["showWinW"]);
                BeginStoryboard((Storyboard)Resources["showWinP"]);
            }
            else
            {
                ApplicationMenu.Visibility = Visibility.Collapsed;
                BeginStoryboard((Storyboard)Resources["hideWinW"]);
                BeginStoryboard((Storyboard)Resources["hideWinP"]);

                Task.Run(() =>
                {
                    Thread.Sleep(500);
                    Dispatcher.BeginInvoke(ShowPanelDelegate, new object[] { });
                });
            }

            state = !state;
            //

        }

    }
}
