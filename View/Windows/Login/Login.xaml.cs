﻿using SiRISApp.View.Windows;
using SiRISApp.ViewModel;
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

namespace SiRISApp.View
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        public LoginVM? LoginVM { get; set; }
        public Login()
        {
            InitializeComponent();
            if (Resources["vm"] != null)
            {
                LoginVM = Resources["vm"] as LoginVM;
                if (LoginVM != null)
                    LoginVM.Authenticated += ViewModel_Authenticated;
            }

        }

        private void ViewModel_Authenticated(object? sender, EventArgs e)
        {
            SiRIS siris = new SiRIS();
            siris.Show();
            Close();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void Minimizar_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void Fechar_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}