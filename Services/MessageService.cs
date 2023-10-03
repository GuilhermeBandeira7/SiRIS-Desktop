using Humanizer;
using MahApps.Metro.Actions;
using SiRISApp.Model;
using SiRISApp.View.Windows.SiRIS;
using SiRISApp.ViewModel.SiRIS;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using static NuGet.Packaging.PackagingConstants;
using static System.Net.Mime.MediaTypeNames;

namespace SiRISApp.Services
{
    public class MessageService
    {
        #region SINGLETON

        private MessageService() { }

        private static MessageService? instance = null;
        public static MessageService Instance
        {
            get
            {
                if (instance == null)
                    instance = new();

                return instance;
            }
        }

        #endregion

        MessageViewModel viewModel = new();
        MessageModel messageModel = new();
        Message message = new();

        bool step = false;

        public void Init()
        {
            viewModel = new(messageModel);
            message = new();
            message.SetContext(viewModel);
        }

        public void Show(string type = "success", string text = "sucess", bool buttons = false, bool loading = false, int min = 0, int max = 100, string image = "")
        {
            messageModel = new()
            {
                Type = type,
                Text = text,
                Buttons = buttons,
                Loading = loading,
                Min = min,
                Max = max,
                Image = image
            };

            if (!buttons && !loading)
            {
                Thread thread = new Thread(RunStatus);
                thread.SetApartmentState(ApartmentState.STA);
                thread.Start();
            }

            if (loading)
            {
                Thread thread = new Thread(RunProgress);
                thread.SetApartmentState(ApartmentState.STA);
                thread.Start();
            }

            Thread.Sleep(500);


        }

        public bool ShowDialog(string type = "success", string text = "sucess", bool buttons = false, bool loading = false, int min = 0, int max = 100, string image = "")
        {
            messageModel = new()
            {
                Type = type,
                Text = text,
                Buttons = buttons,
                Loading = loading,
                Min = min,
                Max = max,
                Image = image
            };

            Init();
            bool resultValue = false;

            if (buttons)
            {
                bool? result = message.ShowDialog();
                if (result == null)
                    resultValue = false;
                else
                    resultValue = (bool)result;
            }
            else
            {
                Task.Run(() =>
                {
                    int charCount = messageModel.Text.Length;
                    int timePerChar = charCount * 50;
                    int sleepTime = (timePerChar) / 100;
                    if (sleepTime < 20) { sleepTime = 20; }

                    while (viewModel.StatusBarValue < 100)
                    {
                        message.Dispatcher.Invoke((Action)(() =>
                        {
                            viewModel.StatusBarValue++;
                        }));

                        Thread.Sleep(sleepTime);
                    }

                    message.Dispatcher.Invoke((Action)(() =>
                    {
                        message.DialogResult = true;
                        message.Close();
                    }));

                });

                bool? result = message.ShowDialog();
                if (result == null)
                    resultValue = false;
                else
                    resultValue = (bool)result;
            }

            return resultValue;
        }

        public void RunStatus()
        {
            Init();
            message.Show();

            Task.Run(() =>
            {
                int charCount = messageModel.Text.Length;
                int timePerChar = charCount * 50;
                int sleepTime = (timePerChar) / 100;
                if (sleepTime < 20) { sleepTime = 20; }

                while (viewModel.StatusBarValue < 100)
                {
                    message.Dispatcher.Invoke((Action)(() =>
                    {
                        viewModel.StatusBarValue++;
                    }));

                    Thread.Sleep(sleepTime);
                }

                message.Dispatcher.Invoke((Action)(() =>
                {
                    message.Close();
                }));

            });

            message.Closed += (sender2, e2) => message.Dispatcher.InvokeShutdown();
            System.Windows.Threading.Dispatcher.Run();
            viewModel.RunStatusMessage();
            message.Close();
        }

        public void RunProgress()
        {
            Init();
            message.Show();
            step = false;
            viewModel.ProgressText = $"{viewModel.Minimum}/{viewModel.Maximum}";

            Task.Run(() =>
            {
                while (viewModel.ProgressBarValue < 100)
                {
                    Thread.Sleep(100);
                }

                message.Dispatcher.Invoke((Action)(() =>
                {
                    message.Close();
                }));

            });

            message.Closed += (sender2, e2) => message.Dispatcher.InvokeShutdown();
            System.Windows.Threading.Dispatcher.Run();
        }

        public void Step(string text = "", string image = "")
        {
            if (text != string.Empty)
                viewModel.Text = text;

            if (image != string.Empty)
                viewModel.Image = image;

            message.Dispatcher.Invoke((Action)(() =>
            {
                viewModel.StepProgressBar();
            }));
            Thread.Sleep(500);
        }
    }
}
