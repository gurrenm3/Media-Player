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
        bool waiting = false;
        public VideoPlayer()
        {
            InitializeComponent();
            ControlsPanel.Visibility = Visibility.Hidden;
        }

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
            Player.LoadedBehavior = Player.IsPlaying() ? MediaState.Pause : MediaState.Play;
        }

        private void ForwardTen_Click(object sender, RoutedEventArgs e)
        {

        }

        private void RewindTen_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
