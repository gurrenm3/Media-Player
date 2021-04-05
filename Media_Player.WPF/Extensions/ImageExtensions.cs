using System;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Bitmap = System.Drawing.Bitmap;

namespace Media_Player.Extensions
{
    public static class ImageExtensions
    {
        public static void SetSource(this Image image, string filePath)
        {
            image.SetSource(new BitmapImage(new Uri(filePath)));
        }

        public static void SetSource(this Image image, BitmapImage bitmapImage)
        {
            image.Source = bitmapImage;
        }

        public static void SetSource(this Image image, Bitmap bitmap)
        {
            image.SetSource(bitmap.ToBitmapImage());
        }
    }
}
