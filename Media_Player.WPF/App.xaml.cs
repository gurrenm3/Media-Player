using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Unosquare.FFME;

namespace Media_Player.WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public const string applicationName = "Media Player";

        public App()
        {
            if (Directory.Exists(@"../ffmpeg")) // ffmpeg folder in same folder as executable
                Library.FFmpegDirectory = @"../ffmpeg";

            else if (Directory.Exists(@"../../../../ffmpeg")) // ffmpeg folder is in project solution folder
                Library.FFmpegDirectory = @"../../../../ffmpeg";

            else if (Directory.Exists(@"c:\ffmpeg" + (Environment.Is64BitProcess ? @"\x64" : string.Empty))) // ffmpeg folder is downloaded to C drive
                Library.FFmpegDirectory = @"c:\ffmpeg" + (Environment.Is64BitProcess ? @"\x64" : string.Empty);

            else
                throw new DirectoryNotFoundException($"Can't run {applicationName} because ffmpeg was not found!");
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            //Current.MainWindow = new MainWindow();
            //Current.MainWindow.Loaded += (snd, eva) => ViewModel.OnApplicationLoaded();
            //Current.MainWindow.Show();

            // Pre-load FFmpeg libraries in the background. This is optional.
            // FFmpeg will be automatically loaded if not already loaded when you try to open
            // a new stream or file. See issue #242
            Task.Run(async () =>
            {
                try
                {
                    // Pre-load FFmpeg
                    await Library.LoadFFmpegAsync();
                }
                catch (Exception ex)
                {
                    var dispatcher = Current?.Dispatcher;
                    if (dispatcher != null)
                    {
                        await dispatcher.BeginInvoke(new Action(() =>
                        {
                            MessageBox.Show(MainWindow,
                                $"Unable to Load FFmpeg Libraries from path:\r\n    {Library.FFmpegDirectory}" +
                                $"\r\nMake sure the above folder contains FFmpeg shared binaries (dll files) for the " +
                                $"applicantion's architecture ({(Environment.Is64BitProcess ? "64-bit" : "32-bit")})" +
                                $"\r\nTIP: You can download builds from https://ffmpeg.org/download.html" +
                                $"\r\n{ex.GetType().Name}: {ex.Message}\r\n\r\nApplication will exit.",
                                "FFmpeg Error",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);

                            Current?.Shutdown();
                        }));
                    }
                }
            });
        }
    }
}
