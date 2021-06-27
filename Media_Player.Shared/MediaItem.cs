using System;
using System.IO;
using System.Text.Json;

namespace Media_Player.Shared
{
    public class MediaItem
    {
        public MediaThumbnail Thumbnail { get; private set; }

        public MediaItem()
        {

        }
        
        public void SetThumbnail(MediaThumbnail thumbnail)
        {
            Thumbnail = thumbnail;
        }

        public string ToJSON()
        {
            return JsonSerializer.Serialize(this);
        }

        public static T FromJSON<T>(string jsonFilePath) where T : class
        {
            ThrowIfPathIsInvalid(jsonFilePath);
            return JsonSerializer.Deserialize<T>(File.ReadAllText(jsonFilePath));
        }

        protected static void ThrowIfPathIsInvalid(string filePath)
        {
            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
            {
                throw new Exception("Error! Tried to create a MediaItem object with " +
                    "an invalid path Or file that doesn't exist. Attempted path: " + filePath);
            }
        }
    }
}
