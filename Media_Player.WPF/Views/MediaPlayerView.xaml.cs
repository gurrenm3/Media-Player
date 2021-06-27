using Media_Player.Extensions;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Media_Player.WPF.Views
{
    /// <summary>
    /// Interaction logic for MediaPlayerView.xaml
    /// </summary>
    public partial class MediaPlayerView : UserControl
    {
        public bool IsMediaPlaying { get { return MediaPlayer.IsPlaying; } }
        string debugMediaPath = @"H:\So I'm a Spider, So What - 12.mp4";

        double _heightBeforeFullscreen;
        double _widthBeforeFullscreen;
        bool _isTimeSliderHeld;

        // these control the autoshow/autohiding of cursor and UI when mouse movement is detected
        bool _waiting;
        int _currentWaitTime;
        const int defaultWaitTime = 1500;
        // =======


        public MediaPlayerView()
        {
            InitializeComponent();
            HideControls(false);
            MediaPlayer.AllowDrop = true;
            MediaPlayer.IsMuted = true;
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            MainWindow.instance.OnEnterFullscreen += MainWindow_OnEnterFullscreen;
            MainWindow.instance.OnExitFullscreen += MainWindow_OnExitFullscreen;
            MainWindow.instance.MouseMove += MainWindow_MouseMove;
            MainWindow.instance.Drop += MediaDropped_Drop;
            VolumeController.OnVolumeChanged += VolumeController_OnVolumeChanged;
            VolumeController.OnMuteChanged += VolumeController_OnMuteChanged;
            MediaPlayer.MediaOpened += MediaPlayer_MediaOpened;
            MediaPlayer.PositionChanged += MediaPlayer_PositionChanged;

            if (File.Exists(debugMediaPath))
            {
                await MediaPlayer.Open(debugMediaPath);
            }
        }

        public void HideControls(bool hideCursor = true)
        {
            ControlsGrid.Visibility = Visibility.Hidden;
            if (hideCursor && MainWindow.instance != null)
                MainWindow.instance.Cursor = Cursors.None;
        }

        public void ShowControls(bool showCursor = true)
        {
            ControlsGrid.Visibility = Visibility.Visible;
            if (showCursor && MainWindow.instance != null)
                MainWindow.instance.Cursor = Cursors.Arrow;
        }

        public bool AreControlsVisible(bool includeCursor = true)
        {
            bool areVisible = ControlsGrid.Visibility == Visibility.Visible;
            return includeCursor ? (areVisible && MainWindow.instance.Cursor == Cursors.Arrow) : areVisible;
        }

        private void MediaPlayer_PositionChanged(object sender, Unosquare.FFME.Common.PositionChangedEventArgs e)
        {
            if (_isTimeSliderHeld)
                return;

            var newValue = e.Position.TotalSeconds / MediaPlayer.MediaInfo.Duration.TotalSeconds;
            TimeSlider.Value = newValue;
        }

        private void MediaPlayer_MediaOpened(object sender, Unosquare.FFME.Common.MediaOpenedEventArgs e)
        {
            ((Image)PlayButton.Content).SetSource(Properties.Resources.pause_icon);
        }

        public async Task Play()
        {
            await MediaPlayer.Play();
            ((Image)PlayButton.Content).SetSource(Properties.Resources.pause_icon);
        }

        public async Task Pause()
        {
            await MediaPlayer.Pause();
            ((Image)PlayButton.Content).SetSource(Properties.Resources.play_arrow);
        }


        private void VolumeController_OnMuteChanged(object sender, bool e)
        {
            MediaPlayer.IsMuted = e;
        }

        private void VolumeController_OnVolumeChanged(object sender, double e)
        {
            if (MediaPlayer.IsMuted)
                MediaPlayer.IsMuted = false;

            MediaPlayer.Volume = e;
        }

        private async void MediaDropped_Drop(object sender, DragEventArgs e)
        {
            var files = (string[])e.Data.GetData(DataFormats.FileDrop);
            string file = files[0];
            await MediaPlayer.Open(file);
        }


        private async void MainWindow_MouseMove(object sender, MouseEventArgs e)
        {
            _currentWaitTime = defaultWaitTime;
            if (_waiting || ControlsPanel.IsMouseCaptured)
                return;

            _waiting = true;
            ShowControls();
            await Task.Factory.StartNew(() =>
            {
                while (_currentWaitTime > 0)
                {
                    Thread.Sleep(1);
                    _currentWaitTime--;
                }
            });

            HideControls();
            _waiting = false;
        }


        private async void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            if (IsMediaPlaying)
                await Pause();
            else
                await Play();
        }

        private void ForwardTen_Click(object sender, RoutedEventArgs e)
        {
            MediaPlayer.Position = MediaPlayer.Position.Add(seconds: 10);
        }

        private void RewindTen_Click(object sender, RoutedEventArgs e)
        {
            MediaPlayer.Position = MediaPlayer.Position.Subtract(seconds: 10);
        }

        private void MainWindow_OnEnterFullscreen(object sender, EventArgs e)
        {
            _heightBeforeFullscreen = MediaPlayer.Height;
            _widthBeforeFullscreen = MediaPlayer.Width;

            MediaPlayer.Height = MainWindow.instance.ActualHeight;
            MediaPlayer.Width = MainWindow.instance.ActualWidth;
            ((Image)FullscreenButton.Content).SetSource(Properties.Resources.ExitFullscreen_icon);
        }

        private void MainWindow_OnExitFullscreen(object sender, EventArgs e)
        {
            MediaPlayer.Height = _heightBeforeFullscreen;
            MediaPlayer.Width = _widthBeforeFullscreen;
            ShowControls();
            ((Image)FullscreenButton.Content).SetSource(Properties.Resources.Fullscreen_icon);
        }

        private void MediaPlayer_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            MainWindow.instance.Fullscreen = true;
        }

        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            TimeSlider.Width = ActualWidth - 25;
        }

        bool changingTime = false;
        private async void TimeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!_isTimeSliderHeld)
                return;

            if (changingTime)
                return;

            changingTime = true;

            await Task.Factory.StartNew(() => Thread.Sleep(5));

            var mediaLength = MediaPlayer.MediaInfo.Duration.TotalSeconds;
            var newLength = mediaLength * TimeSlider.Value;

            MediaPlayer.Position = new TimeSpan().Add(seconds: (int)newLength);
            changingTime = false;
        }

        bool replayOnMouseUp;
        private async void TimeSlider_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            _isTimeSliderHeld = true;


            if (MediaPlayer.IsPlaying)
            {
                await Pause();
                replayOnMouseUp = true;
            }
        }

        private async void TimeSlider_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            _isTimeSliderHeld = false;
            if (replayOnMouseUp)
            {
                await Play();
                replayOnMouseUp = false;
            }
        }

        private void FullscreenButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.instance.Fullscreen = !MainWindow.instance.Fullscreen;
            MediaPlayer.Focus();
        }
    }
}
