using Media_Player.Shared;
using System;
using System.Threading.Tasks;
using Unosquare.FFME;

namespace Media_Player.Extensions
{
    public static class MediaElementExtensions
    {
        public static async Task Open(this MediaElement mediaElement, string filepath)
        {
            await mediaElement.Open(new Uri(filepath));
        }

        public static string TryGetTitle(this MediaElement mediaElement)
        {
            mediaElement.Metadata.TryGetValue("title", out string title);
            return title;
        }

        public static string TryGetEncoder(this MediaElement mediaElement)
        {
            mediaElement.Metadata.TryGetValue("encoder", out string encoder);
            return encoder;
        }
    }
}
