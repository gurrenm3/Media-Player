using System.Drawing;

namespace Media_Player.Shared
{
    public class MediaThumbnail
    {
        public string ImageFilePath { get; private set; }
        private Bitmap imageToBitmap;
        private bool imageHasChanged;

        public MediaThumbnail(string imageFilePath)
        {
            ImageFilePath = imageFilePath;
        }

        public void SetImagePath(string newPath)
        {
            ImageFilePath = newPath;
            imageHasChanged = true;
        }

        public Bitmap ToBitmap()
        {
            if (imageToBitmap != null && !imageHasChanged)
                return imageToBitmap;

            imageToBitmap = new Bitmap(ImageFilePath);
            imageHasChanged = false; // reset image so it's not changed
            return imageToBitmap;
        }
    }
}
