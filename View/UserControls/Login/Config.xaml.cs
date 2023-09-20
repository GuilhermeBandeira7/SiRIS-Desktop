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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SiRISApp.View.UserControls.Login
{
    /// <summary>
    /// Interaction logic for LoginConfig.xaml
    /// </summary>
    public partial class LoginConfig : UserControl
    {
        public LoginConfig()
        {
            InitializeComponent();
        }

        private void PasswordBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                LoginButton.Command.Execute(null);
            }
        }

        private void NewServer_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                CreateServerCommand.Command.Execute(null);
                CreateServerCloseDialog.Command.Execute(null);
                CreateServerMoveFirst.Command.Execute(null);
            }
        }
    }
}
