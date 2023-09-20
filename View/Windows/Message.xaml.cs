using SiRISApp.ViewModel.SiRIS;
using System.Threading;
using System.Windows;

namespace SiRISApp.View.Windows.SiRIS
{
    /// <summary>
    /// Interaction logic for Message.xaml
    /// </summary>
    public partial class Message : Window
    {
        public delegate void CloseWindow();
        public CloseWindow closeWindowCallback;
        public bool userInput = false;

        public Message()
        {
            InitializeComponent();
            closeWindowCallback = new CloseWindow(CloseWindowsCallback);
        }


        public void SetType(string type, string message, bool userInput = false)
        {
            if (userInput)
            {
                ProgressBar.Visibility = Visibility.Collapsed;
                InputGrid.Visibility = Visibility.Visible;
            }

            this.userInput = userInput;

            if (Resources["vm"] != null)
            {
                MessageViewModel viewModel = (MessageViewModel)Resources["vm"];
                viewModel.SetMessage(type, message);
            }
        }

        private void CloseWindowsCallback()
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
            {
                Thread thread = new(() =>
                {
                    if (Resources["vm"] != null)
                    {
                        MessageViewModel viewModel = (MessageViewModel)Resources["vm"];
                        if(!userInput)
                        {
                            int charCount = viewModel.Message.Length;
                            int timePerChar = charCount * 50;
                            int sleepTime = (timePerChar) / 100;
                            if(sleepTime < 50) { sleepTime = 50; }
                            while (viewModel.ProgressBarValue < 100)
                            {
                                viewModel.ProgressBarValue++;
                                Thread.Sleep(sleepTime);
                            }

                            Dispatcher.BeginInvoke(closeWindowCallback, new object[] { });
                        }
                    }
                });

                thread.Start();
    
            }
        }
    }
}
