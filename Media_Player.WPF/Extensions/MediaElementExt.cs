using System;
using System.Threading.Tasks;
using Unosquare.FFME;

namespace Media_Player.Extensions
{
    public static class MediaElementExt
    {
        public static async Task Open(this MediaElement mediaElement, string filepath)
        {
            await mediaElement.Open(new Uri(filepath));
        }
    }
}
