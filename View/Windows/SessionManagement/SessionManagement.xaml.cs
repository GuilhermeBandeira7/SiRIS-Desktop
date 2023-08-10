using SiRISApp.ViewModel.SessionManagement;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SiRISApp.View.Windows
{
    /// <summary>
    /// Interaction logic for SessionManagement.xaml
    /// </summary>
    public partial class SessionManagement : Window
    {
        public SessionManagement()
        {
            InitializeComponent();
        }

        private void Button_MouseEnter(object sender, MouseEventArgs e)
        {
            var viewModel = Resources["vm"] as SessionManagementViewModel;
            if (viewModel != null)
            {
                SessionViewModel sessionViewModel = new SessionViewModel();
                sessionViewModel.ReloadSessions += viewModel.ReloadSessionEvent;
                viewModel.SelectedSession = sessionViewModel;
            }
           
        }
    }
}
