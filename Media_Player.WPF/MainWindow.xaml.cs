﻿using Media_Player.WPF.UserControls;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using Media_Player.Extensions;
using System.Windows.Controls;

namespace Media_Player.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Properties
        public string ApplicationName { get { return App.applicationName; } }

        public bool Fullscreen
        {
            get { return _fullscreen; }
            set { SetFullscreen(value); }
        }
        private bool _fullscreen;
        #endregion

        #region Fields
        public static MainWindow instance;
        //public VideoPlayer player = new VideoPlayer();
        public event EventHandler OnEnterFullscreen;
        public event EventHandler OnExitFullscreen;

        private WindowState _windowStateBeforeFullscreen;
        private WindowStyle _windowStyleBeforeFullscreen;
        #endregion


        public MainWindow()
        {
            InitializeComponent();

            instance = this;
            Logger.MessageLogged += Logger_MessageLogged;
        }

        private void FinishedLoading(object sender, EventArgs e) //Executed when MainWindow.ContentRendered is called
        {
            //ContentGrid.Children.Add(player);
        }


        public static void RunOnUIThread(Action action) => instance.Dispatcher.Invoke(action);

        private void Window_Closing(object sender, CancelEventArgs e) => Application.Current.Shutdown();

        private void Logger_MessageLogged(object sender, Logger.LogEvents e)
        {
            if (e.LogType == Logger.LogType.MessageBox)
                MessageBox.Show(e.Message);

            RunOnUIThread(() =>
            {
                if (e.LogType == Logger.LogType.Console)
                {
                    System.Diagnostics.Debug.WriteLine(e.Message);
                }
            });
        }

        private void GithubButton_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://github.com/gurrenm3/Media-Player");
        }

        private void DiscordButton_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://discord.com/invite/GkdDrGr");
        }

        private void DebugButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private async void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
                await player.MediaPlayer.Pause();

            if (e.Key == Key.Escape && Fullscreen)
                SetFullscreen(false);

            if (e.Key == Key.F11)
                SetFullscreen(!Fullscreen);
        }


        #region Fullscreen Stuff
        private void FullscreenButton_Click(object sender, RoutedEventArgs e)
        {
            SetFullscreen(!Fullscreen);
        }

        private void Window_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.OriginalSource is not Border)
                return;
            SetFullscreen(!Fullscreen);
        }

        private void SetFullscreen(bool isFullscreen)
        {
            if (isFullscreen)
                EnterFullscreen();
            else
                ExitFullscreen();
        }

        private void EnterFullscreen()
        {
            if (Fullscreen)
                return;

            _windowStateBeforeFullscreen = WindowState;
            _windowStyleBeforeFullscreen = WindowStyle;

            Visibility = Visibility.Collapsed;
            Topmost = true;
            WindowStyle = WindowStyle.None;
            ResizeMode = ResizeMode.NoResize;
            Visibility = Visibility.Visible;

            WindowState = WindowState.Maximized;

            ToolbarGrid.Visibility = Visibility.Collapsed;

            _fullscreen = true;
            if (OnEnterFullscreen != null)
                OnEnterFullscreen.Invoke(this, EventArgs.Empty);
        }

        private void ExitFullscreen()
        {
            if (!Fullscreen)
                return;

            Topmost = false;
            WindowState = _windowStateBeforeFullscreen;
            WindowStyle = _windowStyleBeforeFullscreen;
            ResizeMode = ResizeMode.CanResize;

            ToolbarGrid.Visibility = Visibility.Visible;

            _fullscreen = false;
            if (OnExitFullscreen != null)
                OnExitFullscreen.Invoke(this, EventArgs.Empty);
        }

        #endregion

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            player.VolumeController.VolumeSlider.Visibility = Visibility.Hidden;
        }

        private async void player_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (player.MediaPlayer.IsPlaying)
                await player.Pause();
            else
                await player.Play();
        }
    }
}
