using SiRISApp.ViewModel.SiRIS;
using System;
using System.Collections.Generic;
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

namespace SiRISApp.View.Windows.SiRIS
{
    /// <summary>
    /// Interaction logic for Message.xaml
    /// </summary>
    public partial class Message : Window
    {
        public Message()
        {
            InitializeComponent();
        }

        public void SetType(string type, string message)
        {
            if (Resources["vm"] != null)
            {
                MessageViewModel viewModel = (MessageViewModel)Resources["vm"];
                viewModel.SetMessage(type, message);
            }
        }
    }
}
