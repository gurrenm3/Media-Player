using Media_Player.Lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Media_Player.Extensions;
using System.Diagnostics;
using Media_Player.WPF.UserControls;

namespace Media_Player.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public bool IsFullscreen { get; private set; }

        public static MainWindow instance;
        public VideoPlayer player = new VideoPlayer();
        public event EventHandler OnEnterFullscreen;
        public event EventHandler OnExitFullscreen;

        private WindowState _windowStateBeforeFullscreen;
        private WindowStyle _windowStyleBeforeFullscreen;
        
        

        public MainWindow()
        {
            InitializeComponent();
            instance = this;
            Logger.MessageLogged += Logger_MessageLogged;
            ContentRendered += FinishedLoading;
        }

        private void FinishedLoading(object sender, EventArgs e) //Executed when MainWindow.ContentRendered is called
        {
            ContentGrid.Children.Add(player);
        }


        public static void RunOnUIThread(Action action) => instance.Dispatcher.Invoke(action);

        private void Window_Closed(object sender, EventArgs e) => Application.Current.Shutdown();

        private void Logger_MessageLogged(object sender, Logger.LogEvents e)
        {
            if (e.LogType == Logger.LogType.MessageBox)
                MessageBox.Show(e.Message);

            RunOnUIThread(() => 
            {
                if (e.LogType == Logger.LogType.Console)
                {

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

        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Space)
                player.MediaPlayer.PauseMedia();

            if (e.Key == System.Windows.Input.Key.Escape && IsFullscreen)
                ExitFullscreen();
        }

        public void EnterFullscreen()
        {
            if (IsFullscreen)
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

            IsFullscreen = true;
            if (OnEnterFullscreen != null)
                OnEnterFullscreen.Invoke(this, EventArgs.Empty);
        }

        public void ExitFullscreen()
        {
            if (!IsFullscreen)
                return;

            Topmost = false;
            WindowState = _windowStateBeforeFullscreen;
            WindowStyle = _windowStyleBeforeFullscreen;
            ResizeMode = ResizeMode.CanResize;

            ToolbarGrid.Visibility = Visibility.Visible;

            IsFullscreen = false;
            if (OnExitFullscreen != null)
                OnExitFullscreen.Invoke(this, EventArgs.Empty);
        }

        private void FullscreenButton_Click(object sender, RoutedEventArgs e)
        {
            if (!IsFullscreen)
                EnterFullscreen();
            else
                ExitFullscreen();
        }
    }
}
