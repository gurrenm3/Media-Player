using System.Collections.Generic;

namespace Media_Player.Shared
{
    public class MediaSeason : MediaItem
    {
        public MediaSeries ParentSeries { get; private set; }
        public List<MediaEpisode> Episodes { get; set; }

        public MediaSeason(MediaSeries parentSeries)
        {
            ParentSeries = parentSeries;
        }
    }
}
