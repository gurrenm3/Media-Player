using Media_Player.Extensions;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Diagnostics;
using Media_Player.WPF.Extensions;
using Media_Player.WPF.UserControls;
using Unosquare.FFME.Common;
using MediaElement = Unosquare.FFME.MediaElement;

namespace Media_Player.WPF.Views
{
    /// <summary>
    /// Interaction logic for MediaPlayerView.xaml
    /// </summary>
    public partial class MediaPlayerView : UserControl
    {
        public bool IsMediaPlaying { get { return mediaPlayer.IsPlaying; } }
        public bool IsMediaInfoShowing { get { return mediaInfoPopup.Content != null; } }

        //string debugMediaPath = @"H:\So I'm a Spider, So What - 12.mp4";
        string debugMediaPath = @"H:\My Hero Academia\Season 4\My Hero Academia S04 1080p Dual Audio BDRip 10 bits DD x265-EMBER\S04E03-Boy Meets... [1450FC0B].mkv";

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
            mediaPlayer.AllowDrop = true;
            mediaPlayer.IsMuted = true;
            mediaInfoPopup.Visibility = Visibility.Hidden;
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            MainWindow.instance.OnEnterFullscreen += MainWindow_OnEnterFullscreen;
            MainWindow.instance.OnExitFullscreen += MainWindow_OnExitFullscreen;
            MainWindow.instance.MouseMove += MainWindow_MouseMove;
            MainWindow.instance.Drop += MediaDropped_Drop;
            volumeController.OnVolumeChanged += VolumeController_OnVolumeChanged;
            volumeController.OnMuteChanged += VolumeController_OnMuteChanged;
            mediaPlayer.MediaOpened += MediaPlayer_MediaOpened;
            mediaPlayer.PositionChanged += MediaPlayer_PositionChanged;

            if (File.Exists(debugMediaPath))
            {
                await mediaPlayer.Open(debugMediaPath);
            }
        }

        public void HideControls(bool hideCursor = true)
        {
            controlsGrid.Visibility = Visibility.Hidden;
            popupGrid.Visibility = Visibility.Hidden;
            HideMediaInfoPopup();

            if (hideCursor && MainWindow.instance != null)
                MainWindow.instance.Cursor = Cursors.None;
        }

        public void ShowControls(bool showCursor = true)
        {
            controlsGrid.Visibility = Visibility.Visible;
            popupGrid.Visibility = Visibility.Visible;

            if (showCursor && MainWindow.instance != null)
                MainWindow.instance.Cursor = Cursors.Arrow;
        }

        public bool AreControlsVisible(bool includeCursor = true)
        {
            bool areVisible = controlsGrid.Visibility == Visibility.Visible;
            return includeCursor ? (areVisible && MainWindow.instance.Cursor == Cursors.Arrow) : areVisible;
        }

        private void MediaPlayer_PositionChanged(object sender, Unosquare.FFME.Common.PositionChangedEventArgs e)
        {
            if (_isTimeSliderHeld)
                return;

            var newValue = e.Position.TotalSeconds / mediaPlayer.MediaInfo.Duration.TotalSeconds;
            timeSlider.Value = newValue;
        }

        private void MediaPlayer_MediaOpened(object sender, Unosquare.FFME.Common.MediaOpenedEventArgs e)
        {
            ((Image)playButton.Content).SetSource(Properties.Resources.pause_icon);
            mediaInfoPopup.Visibility = Visibility.Visible;
        }

        public async Task Play()
        {
            await mediaPlayer.Play();
            ((Image)playButton.Content).SetSource(Properties.Resources.pause_icon);
        }

        public async Task Pause()
        {
            await mediaPlayer.Pause();
            ((Image)playButton.Content).SetSource(Properties.Resources.play_arrow);
        }


        private void VolumeController_OnMuteChanged(object sender, bool e)
        {
            mediaPlayer.IsMuted = e;
        }

        private void VolumeController_OnVolumeChanged(object sender, double e)
        {
            if (mediaPlayer.IsMuted)
                mediaPlayer.IsMuted = false;

            mediaPlayer.Volume = e;
        }

        private async void MediaDropped_Drop(object sender, DragEventArgs e)
        {
            var files = (string[])e.Data.GetData(DataFormats.FileDrop);
            string file = files[0];
            await mediaPlayer.Open(file);
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
            mediaPlayer.Position = mediaPlayer.Position.Add(seconds: 10);
        }

        private void RewindTen_Click(object sender, RoutedEventArgs e)
        {
            mediaPlayer.Position = mediaPlayer.Position.Subtract(seconds: 10);
        }

        private void MainWindow_OnEnterFullscreen(object sender, EventArgs e)
        {
            _heightBeforeFullscreen = mediaPlayer.Height;
            _widthBeforeFullscreen = mediaPlayer.Width;

            mediaPlayer.Height = MainWindow.instance.ActualHeight;
            mediaPlayer.Width = MainWindow.instance.ActualWidth;
            ((Image)FullscreenButton.Content).SetSource(Properties.Resources.ExitFullscreen_icon);
        }

        private void MainWindow_OnExitFullscreen(object sender, EventArgs e)
        {
            mediaPlayer.Height = _heightBeforeFullscreen;
            mediaPlayer.Width = _widthBeforeFullscreen;
            ShowControls();
            ((Image)FullscreenButton.Content).SetSource(Properties.Resources.Fullscreen_icon);
        }

        private void MediaPlayer_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            MainWindow.instance.Fullscreen = true;
        }

        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            timeSlider.Width = ActualWidth - 25;
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

            var mediaLength = mediaPlayer.MediaInfo.Duration.TotalSeconds;
            var newLength = mediaLength * timeSlider.Value;

            mediaPlayer.Position = new TimeSpan().Add(seconds: (int)newLength);
            changingTime = false;
        }

        bool replayOnMouseUp;
        private async void TimeSlider_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            _isTimeSliderHeld = true;


            if (mediaPlayer.IsPlaying)
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
            mediaPlayer.Focus();
        }

        private void mediaInfoPopup_Click(object sender, RoutedEventArgs e)
        {
            if (mediaPopupBox.Content == null)
            {
                ShowMediaInfoPopup();
            }
            else
            {
                HideMediaInfoPopup();
            }
        }

        public void ShowMediaInfoPopup() => ShowMediaInfoPopup(mediaPlayer.MediaInfo, mediaPlayer);

        public void ShowMediaInfoPopup(MediaInfo mediaInfo, MediaElement player)
        {
            mediaPopupBox.Content = new MediaInfoPopup(mediaInfo, player);
        }

        public void HideMediaInfoPopup()
        {
            mediaPopupBox.Content = null;
        }

        private void UserControl_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            //Debug.WriteLine(e.OriginalSource.GetType().Name);
        }

        private void popupGrid_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            
        }
    }
}
