using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Media_Player.Extensions;

namespace Media_Player.WPF.UserControls
{
    /// <summary>
    /// Interaction logic for VideoPlayer.xaml
    /// </summary>
    public partial class VideoPlayer : UserControl
    {
        public bool IsMediaPlaying { get { return MediaPlayer.IsPlaying(); } }

        private double _heightBeforeFullscreen;
        private double _widthBeforeFullscreen;
        
        public VideoPlayer()
        {
            InitializeComponent();
            ControlsPanel.Visibility = Visibility.Hidden;
        }


        public void PlayMedia() => MediaPlayer.PlayMedia();
        public void PauseMedia() => MediaPlayer.PauseMedia();
        public void StopMedia() => MediaPlayer.StopMedia();


        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            MainWindow.instance.OnEnterFullscreen += MainWindow_OnEnterFullscreen;
            MainWindow.instance.OnExitFullscreen += MainWindow_OnExitFullscreen;
        }


        bool waiting = false;
        private async void UserControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (waiting)
                return;

            waiting = true;
            ControlsPanel.Visibility = Visibility.Visible;
            await Task.Factory.StartNew(() => Thread.Sleep(5000));
            ControlsPanel.Visibility = Visibility.Hidden;
            waiting = false;
        }


        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            if (IsMediaPlaying)
                PauseMedia();
            else
                PlayMedia();

            MainWindow.instance.EnterFullscreen();
        }

        private void ForwardTen_Click(object sender, RoutedEventArgs e)
        {
            MediaPlayer.Position = MediaPlayer.Position.Add(seconds: 10);
        }

        private void RewindTen_Click(object sender, RoutedEventArgs e)
        {
            if (!MediaPlayer.IsPaused())
            {
                MediaPlayer.Position = MediaPlayer.Position.Subtract(seconds: 10);
                return;
            }

            PlayMedia();
            MediaPlayer.Position = MediaPlayer.Position.Subtract(seconds: 10);
            PauseMedia();
        }

        private void MediaPlayer_MediaOpened(object sender, RoutedEventArgs e) // use this to hide unnecessary UI
        {
            //MainWindow.instance.ToolbarGrid.Visibility = Visibility.Collapsed;
        }

        private void MainWindow_OnEnterFullscreen(object sender, EventArgs e)
        {
            _heightBeforeFullscreen = MediaPlayer.Height;
            _widthBeforeFullscreen = MediaPlayer.Width;

            MediaPlayer.Height = MainWindow.instance.ActualHeight;
            MediaPlayer.Width = MainWindow.instance.ActualWidth;
        }

        private void MainWindow_OnExitFullscreen(object sender, EventArgs e)
        {
            MediaPlayer.Height = _heightBeforeFullscreen;
            MediaPlayer.Width = _widthBeforeFullscreen;
        }
    }
}
