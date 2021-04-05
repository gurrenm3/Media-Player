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
using System.Windows.Navigation;
using System.Windows.Shapes;

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

        public VolumeController()
        {
            InitializeComponent();
        }

        private bool IsMediaPlayerMuted()
        {
            return MainWindow.instance.player.MediaPlayer.IsMuted;
        }

        private void VolumeController_MouseEnter(object sender, MouseEventArgs e)
        {
            VolumeSlider.Visibility = Visibility.Visible;
        }

        private void MouseDetection_Rectangle_MouseLeave(object sender, MouseEventArgs e)
        {
            VolumeSlider.Visibility = Visibility.Hidden;
        }

        private void VolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            OnVolumeChanged?.Invoke(this, e.NewValue);
        }

        private void VolumeControl_Button_Click(object sender, RoutedEventArgs e)
        {
            _isMuted = !IsMediaPlayerMuted();
            VolumeSlider.Visibility = Visibility.Hidden;
            OnMuteChanged?.Invoke(this, _isMuted);
        }
    }
}
