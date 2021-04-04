using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Media_Player.Extensions
{
    public static class MediaElementExt
    {
        public static bool IsPlaying(this MediaElement mediaElement)
        {
            return mediaElement.LoadedBehavior == MediaState.Play;
        }

        public static bool IsPaused(this MediaElement mediaElement)
        {
            return mediaElement.LoadedBehavior == MediaState.Pause;
        }

        public static bool IsStopped(this MediaElement mediaElement)
        {
            return mediaElement.LoadedBehavior == MediaState.Stop;
        }

        public static void PlayMedia(this MediaElement mediaElement)
        {
            mediaElement.LoadedBehavior = MediaState.Play;
        }

        public static void PauseMedia(this MediaElement mediaElement)
        {
            mediaElement.LoadedBehavior = MediaState.Pause;
        }

        public static void StopMedia(this MediaElement mediaElement)
        {
            mediaElement.LoadedBehavior = MediaState.Stop;
        }
    }
}
