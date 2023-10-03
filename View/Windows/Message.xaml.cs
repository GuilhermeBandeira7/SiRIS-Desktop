using Humanizer;
using SiRISApp.ViewModel.SiRIS;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace SiRISApp.View.Windows.SiRIS
{
    /// <summary>
    /// Interaction logic for Message.xaml
    /// </summary>
    public partial class Message : Window
    {
        public delegate void CloseWindow(object? sender, EventArgs e);
        public CloseWindow closeWindowCallback;

        public Message()
        {
            InitializeComponent();
            closeWindowCallback = new CloseWindow(CloseWindowsCallback);
        }

        private void CloseWindowsCallback(object? sender, EventArgs e)
        {
            Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();   
        }

        private void Confirm_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void Window_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
                Topmost = true;
        }

        public void SetContext(MessageViewModel viewModel)
        {
            DataContext = viewModel;
        }
    }
}
