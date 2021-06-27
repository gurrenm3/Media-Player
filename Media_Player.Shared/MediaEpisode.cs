namespace Media_Player.Shared
{
    public class MediaEpisode : MediaItem
    {
        public MediaSeries ParentSeries { get; private set; }
        public MediaSeason ParentSeason { get; private set; }
        public string EpisodeFilePath { get; private set; }

        public string EpisodeName { get; set; }
        public string EpisodeDescription { get; set; }
        public string EpisodeLanguage { get; set; }
        public float EpisodeLength { get; set; }

        private MediaEpisode() { }

        private MediaEpisode(string episodePath)
        {
            ThrowIfPathIsInvalid(episodePath);
            EpisodeFilePath = episodePath;
        }

        public MediaEpisode(MediaSeries parentSeries, string episodePath) : this (episodePath)
        {
            ParentSeries = parentSeries;
        }

        public MediaEpisode(MediaSeason parentSeason, string episodePath) : this(episodePath)
        {
            ParentSeason = parentSeason;
        }
    }
}
