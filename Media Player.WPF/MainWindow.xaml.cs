using Media_Player.Lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Media_Player.Extensions;
using System.Diagnostics;

namespace Media_Player.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static MainWindow instance;

        public MainWindow()
        {
            InitializeComponent();
            instance = this;
            Logger.MessageLogged += Logger_MessageLogged;
            ContentRendered += FinishedLoading;
        }

        private void FinishedLoading(object sender, EventArgs e) //Executed when MainWindow.ContentRendered is called
        {
            
        }


        public static void RunOnUIThread(Action action) => instance.Dispatcher.Invoke(action);

        private void Window_Closed(object sender, EventArgs e) => Application.Current.Shutdown();

        private void Logger_MessageLogged(object sender, Logger.LogEvents e)
        {
            if (e.LogType == Logger.LogType.MessageBox)
                MessageBox.Show(e.Message);

            RunOnUIThread(() => 
            {
                if (e.LogType == Logger.LogType.Console)
                {

                }
            });
        }

        private void GithubButton_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://github.com/gurrenm3/Media-Player");
        }

        private void DiscordButton_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://discord.com/invite/GkdDrGr");
        }

        private void DebugButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
