using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Media_Player.Extensions;

namespace Media_Player.WPF.UserControls
{
    /// <summary>
    /// Interaction logic for VolumeController.xaml
    /// </summary>
    public partial class VolumeController : UserControl
    {
        public double VolumeLevel { get { return VolumeSlider.Value; } }
        public event EventHandler<double> OnVolumeChanged;
        public event EventHandler<bool> OnMuteChanged;
        private bool _isMuted;
        private bool _isLoaded;

        public VolumeController()
        {
            InitializeComponent();
        }

        private bool IsMediaPlayerMuted()
        {
            var player = MainWindow.instance?.mediaPlayerView?.MediaPlayer;
            return player is null ? false : player.IsMuted;
        }

        private void VolumeController_MouseEnter(object sender, MouseEventArgs e)
        {
            VolumeSlider.Visibility = Visibility.Visible;
        }

        private void VolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (VolumeControl_Button?.Content is null)
                return;

            var newImage = e.NewValue == 0 ? Properties.Resources.Sound_muted : Properties.Resources.audio_icon_edited;
            ((Image)VolumeControl_Button.Content).SetSource(newImage);

            OnVolumeChanged?.Invoke(this, e.NewValue);
        }

        private void VolumeControl_Button_Click(object sender, RoutedEventArgs e)
        {
            _isMuted = !IsMediaPlayerMuted();

            var newImage = _isMuted ? Properties.Resources.Sound_muted : Properties.Resources.audio_icon_edited;
            ((Image)VolumeControl_Button.Content).SetSource(newImage);

            VolumeSlider.Visibility = Visibility.Hidden;
            OnMuteChanged?.Invoke(this, _isMuted);

        }

        private void UserControl_MouseLeave(object sender, MouseEventArgs e)
        {
            VolumeSlider.Visibility = Visibility.Hidden;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            var newImage = IsMediaPlayerMuted() ? Properties.Resources.Sound_muted : Properties.Resources.audio_icon_edited;
            ((Image)VolumeControl_Button.Content).SetSource(newImage);
        }
    }
}
