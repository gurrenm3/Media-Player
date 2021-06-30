using Media_Player.Shared;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Documents;
using Unosquare.FFME.Common;

namespace Media_Player.WPF.Extensions
{
    public static class MediaInfoExtensions
    {
        public static string TryGetTitle(this MediaInfo mediaInfo)
        {
            mediaInfo.Metadata.TryGetValue("title", out string title);
            return title;
        }

        public static string TryGetEncoder(this MediaInfo mediaInfo)
        {
            mediaInfo.Metadata.TryGetValue("encoder", out string encoder);
            return encoder;
        }

        public static MediaEpisode ToMediaEpisode(this MediaInfo mediaInfo)
        {
            if (string.IsNullOrEmpty(mediaInfo?.MediaSource))
                return null;

            var episode = new MediaEpisode(mediaInfo.MediaSource);
            episode.AddLanguages(GetLanguages(mediaInfo));
            return episode;
        }

        public static List<string> GetLanguages(this MediaInfo mediaInfo)
        {
            if (mediaInfo.Streams.Count == 0)
                return null;

            List<string> languages = new List<string>();
            foreach (var stream in mediaInfo.Streams)
            {
                string language = stream.Value.Language;
                if (string.IsNullOrEmpty(language))
                    continue;

                if (languages.Contains(language))
                    continue;

                languages.Add(language);
            }

            return languages;
        }

        public static string GetSelectedAudio(this MediaInfo mediaInfo)
        {
            mediaInfo.BestStreams.TryGetValue(FFmpeg.AutoGen.AVMediaType.AVMEDIA_TYPE_AUDIO, out var selectedStream);
            return selectedStream?.Language;
        }

        public static string GetSelectedSubtitle(this MediaInfo mediaInfo)
        {
            mediaInfo.BestStreams.TryGetValue(FFmpeg.AutoGen.AVMediaType.AVMEDIA_TYPE_SUBTITLE, out var selectedStream);
            return selectedStream?.Language;
        }
    }
}
