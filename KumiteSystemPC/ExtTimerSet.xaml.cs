﻿using ModernWpf.Controls;
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
using WpfScreenHelper;

namespace KumiteSystemPC
{
    /// <summary>
    /// Логика взаимодействия для ExtTimerSet.xaml
    /// </summary>
    public partial class ExtTimerSet : Window
    {
        public ExtTimerSet()
        {
            InitializeComponent();
        }

        System.Diagnostics.Stopwatch stopWatch = new System.Diagnostics.Stopwatch();
        TimeSpan timerTime;
        TimeSpan remainTime = new TimeSpan();

        TimerExt timerExt;

        #region Timer

        void showTime(string min, string sec)
        {
            minTXT.Text = min;
            secTXT.Text = sec;
            if (timerExt != null) { timerExt.timerExtL.Content = $"{min}:{sec}"; }
        }
        public async void controlTime()
        {

            do
            {
                TimeSpan ts = stopWatch.Elapsed;
                string remainTimes = String.Format("{0:00}:{1:00}",
                                                     remainTime.Minutes, remainTime.Seconds);
                string min = String.Format("{0:00}", remainTime.Minutes);
                string sec = String.Format("{0:00}", remainTime.Seconds);
                showTime(min, sec);
                remainTime = timerTime - ts;
                //CurTime = String.Format("{0:00}:{1:00}:{2:00}",
                //ts.Minutes, ts.Seconds, ts.Milliseconds / 10);
                if (remainTime <= TimeSpan.Zero) { stopWatch.Stop(); TimerFinished(); }
                if (remainTime <= TimeSpan.FromSeconds(15) && !atoshibaraku) { AtoshiBaraku(); }
                await Task.Delay(1000);
            } while (stopWatch.IsRunning);

        }
        bool atoshibaraku = false;
        void AtoshiBaraku()
        {
            atoshibaraku = true;
            //if (Properties.Settings.Default.warningPlayer != null) Properties.Settings.Default.warningPlayer.Play();
            minTXT.Foreground = Brushes.DarkRed;
            secTXT.Foreground = Brushes.DarkRed;
            if (timerExt != null) { timerExt.timerExtL.Foreground = Brushes.DarkRed; timerExt.tBorder.BorderBrush = Brushes.DarkRed; }
        }


        #endregion
        void TimerFinished()
        {
            /*if (Properties.Settings.Default.endPlayer != null) { Properties.Settings.Default.endPlayer.Play(); }*/
            stopWatch.Stop();
            startBtn.Content = "Start";
            timerTime = TimeSpan.Zero;
            minTXT.Foreground = Brushes.White;
            secTXT.Foreground = Brushes.White;
            if (timerExt != null) { timerExt.timerExtL.Foreground = Brushes.White; timerExt.tBorder.BorderBrush = Brushes.White; }
        }

        int sec = 0, min = 0, time;

        private void resetBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!stopWatch.IsRunning)
            {
                min = 0; sec = 0;
                time = 0;
                stopWatch.Reset();
                minTXT.Text = String.Format("{0:d2}", min);
                secTXT.Text = String.Format("{0:d2}", sec);
            }
        }

        void showTimerExt()
        {
            List<Screen> sc = new List<Screen>();
            sc.AddRange(Screen.AllScreens);
            timerExt = new TimerExt();
            timerExt.Owner = this;
            timerExt.Show();
            timerExt.Left = (sc[Properties.Settings.Default.ScreenNR].Bounds.Right + sc[Properties.Settings.Default.ScreenNR].Bounds.Left) / 2 - timerExt.Width / 2;
            timerExt.Top = sc[Properties.Settings.Default.ScreenNR].Bounds.Bottom / 2 - timerExt.Height / 2;

        }

        private void closeExtBtn_Click(object sender, RoutedEventArgs e)
        {
            if (timerExt != null) { timerExt.Close(); timerExt = null; }
        }

        private void minTXT_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                try
                {
                    min = Convert.ToInt32(minTXT.Text);
                    time = min * 60 + sec;
                    timerTime = new TimeSpan(0, min, sec);
                }
                catch { DisplayMessageDialog("Warning", "Invalid values are entered!"); minTXT.Text = "0"; }
            }
        }


        private async void DisplayMessageDialog(string caption, string message)
        {
            ContentDialog ServerDialog = new ContentDialog
            {
                Title = caption,
                Content = message,
                CloseButtonText = "Ok"
            };

            ContentDialogResult result = await ServerDialog.ShowAsync();
        }


        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // Kill logical focus
            FocusManager.SetFocusedElement(FocusManager.GetFocusScope(minTXT), null);
            FocusManager.SetFocusedElement(FocusManager.GetFocusScope(secTXT), null);
            // Kill keyboard focus
            Keyboard.ClearFocus();

        }

        private void secTXT_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                try
                {
                    sec = Convert.ToInt32(secTXT.Text);
                    time = min * 60 + sec;
                    timerTime = new TimeSpan(0, min, sec);
                }
                catch { DisplayMessageDialog("Warning", "Invalid values are entered!"); secTXT.Text = "0"; }
            }
        }

        private void startBtn_Click(object sender, RoutedEventArgs e)
        {
            if (timerTime <= TimeSpan.Zero)
            {
                try
                {
                    min = Convert.ToInt32(minTXT.Text);
                    sec = Convert.ToInt32(secTXT.Text);
                    time = min * 60 + sec;
                    timerTime = new TimeSpan(0, min, sec);
                }
                catch { DisplayMessageDialog("Warning", "Invalid values are entered!"); minTXT.Text = "0"; }
            }

            if (!stopWatch.IsRunning && timerTime > TimeSpan.Zero)
            {
                if (timerExt == null) { showTimerExt(); }
                remainTime = timerTime;
                startBtn.Content = "Stop";
                stopWatch.Start();
                controlTime();
            }
            else if (stopWatch.IsRunning) { stopWatch.Stop(); timerTime = remainTime; startBtn.Content = "Start"; }
        }
    }
}