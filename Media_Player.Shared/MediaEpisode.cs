using System.Collections.Generic;
using System.IO;
using System.Windows.Documents;

namespace Media_Player.Shared
{
    public class MediaEpisode : MediaItem
    {
        private static Dictionary<string, string> languageAbbreviations = new Dictionary<string, string>()
        {
            {"und", "Undefined" }, {"eng","English"}, {"jpn", "Japanese"}
        };

        public MediaSeries ParentSeries { get; private set; }
        public MediaSeason ParentSeason { get; private set; }
        public string EpisodeFilePath { get; private set; }
        public List<string> Languges { get; private set; } = new List<string>();

        public string EpisodeName { get; set; }
        public string EpisodeDescription { get; set; }
        public string EpisodeLanguage { get; set; }
        public float EpisodeLength { get; set; }

        private MediaEpisode() { }

        public MediaEpisode(string episodePath)
        {
            ThrowIfPathIsInvalid(episodePath);
            EpisodeFilePath = episodePath;
            EpisodeName = new FileInfo(episodePath).Name;
        }

        public MediaEpisode(MediaSeries parentSeries, string episodePath) : this (episodePath)
        {
            ParentSeries = parentSeries;
        }

        public MediaEpisode(MediaSeason parentSeason, string episodePath) : this(episodePath)
        {
            ParentSeason = parentSeason;
        }

        public void AddLanguage(string language)
        {
            Languges.Add(language);
            Languges.Sort();
        }

        public void AddLanguages(List<string> languages)
        {
            if (languages == null)
            {
                System.Diagnostics.Debug.WriteLine("Can't add languages, it is null");
                return;
            }
            Languges.AddRange(languages);
            Languges.Sort();
        }

        public string GetLanguageFullName(string abbreviatedLanguage)
        {
            string fullLangName = "";
            if (languageAbbreviations.ContainsKey(abbreviatedLanguage))
            {
                languageAbbreviations.TryGetValue(abbreviatedLanguage, out fullLangName);
            }
            else if (languageAbbreviations.ContainsValue(abbreviatedLanguage))
            {
                fullLangName = abbreviatedLanguage;
            }
            

            if (string.IsNullOrEmpty(fullLangName))
                throw new System.Exception("Error! That language wasn't found! Tried to get the full name for the " +
                    "language abbreviated as: " + abbreviatedLanguage);

            return fullLangName;
        }

        public string GetLanguageAbbreviation(string langFullName)
        {
            string langAbbreviation = "";

            if (languageAbbreviations.ContainsKey(langFullName))
            {
                langAbbreviation = langFullName;
            }
            else if (languageAbbreviations.ContainsValue(langFullName))
            { 
                languageAbbreviations.TryGetValue(langFullName, out langAbbreviation);
            }

            if (string.IsNullOrEmpty(langAbbreviation))
                throw new System.Exception("Error! That language wasn't found! Tried to get the full name for the " +
                    "language abbreviated as: " + langFullName);

            return langAbbreviation;
        }
    }
}
