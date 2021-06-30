using Media_Player.Shared;
using Media_Player.WPF.Extensions;
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
using Unosquare.FFME.Common;
using MediaElement = Unosquare.FFME.MediaElement;

namespace Media_Player.WPF.UserControls
{
    /// <summary>
    /// Interaction logic for MediaInfoPopup.xaml
    /// </summary>
    public partial class MediaInfoPopup : UserControl
    {
        MediaEpisode episode;
        MediaElement player;
        string selectedlang = "";

        private MediaInfoPopup()
        {
            InitializeComponent();
        }

        public MediaInfoPopup(MediaInfo currentMedia, MediaElement mediaPlayer) : this()
        {
            episode = currentMedia.ToMediaEpisode();
            ShowEpisodeInfo(episode);

            player = mediaPlayer;
            mediaPlayer.MediaChanging += MediaPlayer_MediaChanging;
        }

        private void ShowEpisodeInfo(MediaEpisode episode)
        {
            if (episode == null)
                return;

            mediaTitleTxt.Text = episode.EpisodeName;

            languangesComboBox.Items.Clear();
            if (episode.Languges != null && episode.Languges.Count > 0)
            {
                languangesComboBox.Items.Clear();
                episode.Languges.ForEach(language => languangesComboBox.Items.Add(episode.GetLanguageFullName(language)));
            }
            else
            {
                languangesComboBox.Items.Add("Default");
            }

            languangesComboBox.SelectedIndex = 0;
            selectedlang = languangesComboBox.SelectedItem.ToString();
        }

        private void languangesComboBox_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            languangesComboBox.Foreground = languangesComboBox.IsDropDownOpen ? Brushes.White : Brushes.Black;
        }

        private void languangesComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (player is null)
                return;

            if (languangesComboBox.SelectedIndex < 0)
                return;

            selectedlang = languangesComboBox.SelectedItem.ToString();

            player.ChangeMedia();
        }

        private void MediaPlayer_MediaChanging(object sender, MediaOpeningEventArgs e)
        {
            HandleChangeAudioLanguage(e);
        }

        private void HandleChangeAudioLanguage(MediaOpeningEventArgs e)
        {
            string currentLang = e.Info.GetSelectedAudio();
            if (currentLang.ToLower() == selectedlang.ToLower())
                return;

            string langAbbreviation = episode.GetLanguageAbbreviation(selectedlang);
            var newStream = e.Info.Streams.FirstOrDefault(stream => stream.Value.Language == langAbbreviation);

            if (newStream.Value == null)
                throw new Exception("failed to find audio stream for the language: " + langAbbreviation);

            e.Options.AudioStream = newStream.Value;
        }
    }
}
