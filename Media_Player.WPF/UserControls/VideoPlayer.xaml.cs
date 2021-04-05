using Media_Player.Extensions;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace Media_Player.WPF.UserControls
{
    /// <summary>
    /// Interaction logic for VideoPlayer.xaml
    /// </summary>
    public partial class VideoPlayer : UserControl
    {
        public bool IsMediaPlaying { get { return MediaPlayer.IsPlaying; } }

        private double _heightBeforeFullscreen;
        private double _widthBeforeFullscreen;

        // these control the autoshow/autohiding of cursor and UI when mouse movement is detected
        bool _waiting;
        int _currentWaitTime;
        const int defaultWaitTime = 1500;
        // =======

        public VideoPlayer()
        {
            InitializeComponent();
            ControlsPanel.Visibility = Visibility.Hidden;
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

            await MediaPlayer.Open(@"H:\So I'm a Spider, So What - 12.mp4");
        }

        private void MediaPlayer_PositionChanged(object sender, Unosquare.FFME.Common.PositionChangedEventArgs e)
        {
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
            MainWindow.instance.Cursor = Cursors.Arrow;
            ControlsPanel.Visibility = Visibility.Visible;
            await Task.Factory.StartNew(() => 
            {
                while (_currentWaitTime > 0)
                {
                    Thread.Sleep(1);
                    _currentWaitTime--;
                }
            });

            ControlsPanel.Visibility = Visibility.Hidden;
            MainWindow.instance.Cursor = Cursors.None;
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

        private async void RewindTen_Click(object sender, RoutedEventArgs e)
        {
            MediaPlayer.Position = MediaPlayer.Position.Subtract(seconds: 10);
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

        private void VolumeControl_Button_Click(object sender, RoutedEventArgs e)
        {
            //VolumeControl_Popup.PopupMode = MaterialDesignThemes.Wpf.PopupBoxPopupMode.MouseOver;
        }

        private void VolumeController_MouseLeave(object sender, MouseEventArgs e)
        {
            /*var volumeController = (VolumeController)sender;
            volumeController.VolumeSlider.Visibility = Visibility.Hidden;*/
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
            /*if (changingTime)
                return;

            changingTime = true;

            await Task.Factory.StartNew(() => Thread.Sleep(5));

            var mediaLength = MediaPlayer.MediaInfo.Duration.TotalSeconds;
            var newLength = mediaLength * TimeSlider.Value;

            MediaPlayer.Position = new TimeSpan().Add(seconds: (int)newLength);
            changingTime = false;*/
        }

        private async void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            /*if (MediaPlayer.IsPlaying)
                await Pause();
            else
                await Play();*/
        }

        private async void MediaPlayer_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            Debug.WriteLine(e.OriginalSource.GetType());
            if (sender is Button)
                return;

            if (MediaPlayer.IsPlaying)
                await Pause();
            else
                await Play();
        }

        bool replayOnMouseUp;
        private async void TimeSlider_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (MediaPlayer.IsPlaying)
            {
                await Pause();
                replayOnMouseUp = true;
            }
        }

        private async void TimeSlider_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (replayOnMouseUp)
            {
                await Play();
                replayOnMouseUp = false;
            }
        }
    }
}