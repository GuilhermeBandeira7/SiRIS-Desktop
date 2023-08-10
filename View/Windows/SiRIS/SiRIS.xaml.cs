using System;
using System.Windows;
using System.Windows.Shapes;
using System.IO;
using System.Reflection;
using SiRISApp.ViewModel;
using EntityMtwServer.Entities;

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

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
