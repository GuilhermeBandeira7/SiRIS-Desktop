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

        MessageModel messageModel = new();

        MessageViewModel ViewModel { get; set; }   
        Message Message { get; set; }

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

            ViewModel = new(messageModel);
            Message = new();
            Message.SetContext(ViewModel);

            bool resultValue = false;

            if (buttons)
            {
                bool? result = Message.ShowDialog();
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

                    while (ViewModel.StatusBarValue < 100)
                    {
                        Message.Dispatcher.Invoke((Action)(() =>
                        {
                            ViewModel.StatusBarValue++;
                        }));

                        Thread.Sleep(sleepTime);
                    }

                    Message.Dispatcher.Invoke((Action)(() =>
                    {
                        Message.DialogResult = true;
                        Message.Close();
                    }));

                });

                bool? result = Message.ShowDialog();
                if (result == null)
                    resultValue = false;
                else
                    resultValue = (bool)result;
            }

            return resultValue;
        }

        public void RunStatus()
        {

            ViewModel = new(messageModel);
            Message = new();
            Message.SetContext(ViewModel);
            Message.Show();

            Task.Run(() =>
            {
                int charCount = messageModel.Text.Length;
                int timePerChar = charCount * 50;
                int sleepTime = (timePerChar) / 100;
                if (sleepTime < 20) { sleepTime = 20; }

                while (ViewModel.StatusBarValue < 100)
                {
                    Message.Dispatcher.Invoke((Action)(() =>
                    {
                        ViewModel.StatusBarValue++;
                    }));

                    Thread.Sleep(sleepTime);
                }

                Message.Dispatcher.Invoke((Action)(() =>
                {
                    Message.Close();
                }));

            });

            Message.Closed += (sender2, e2) => Message.Dispatcher.InvokeShutdown();
            System.Windows.Threading.Dispatcher.Run();
          
            if (Message.IsActive)
                Message.Close();
        }

        public void RunProgress()
        {
            ViewModel = new(messageModel);
            Message = new();
            Message.SetContext(ViewModel);
            Message.Show();
            ViewModel.ProgressText = $"{ViewModel.Minimum}/{ViewModel.Maximum}";

            Task.Run(() =>
            {
                while (ViewModel.ProgressBarValue < 100)
                {
                    Thread.Sleep(100);
                }

                Message.Dispatcher.Invoke((Action)(() =>
                {
                    Message.Close();
                }));
            });

            Message.Closed += (sender2, e2) => Message.Dispatcher.InvokeShutdown();
            System.Windows.Threading.Dispatcher.Run();
        }

        public void Step(string text = "", string image = "")
        {
            if (text != string.Empty)
                ViewModel.Text = text;

            if (image != string.Empty)
                ViewModel.Image = image;

            Message.Dispatcher.Invoke((Action)(() =>
            {
                ViewModel.StepProgressBar();
            }));

            Thread.Sleep(500);
        }
    }
}
