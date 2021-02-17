using Media_Player.Lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

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
        }


        public static void RunOnUIThread(Action action) => instance.Dispatcher.Invoke(action);
        private void Window_ContentRendered(object sender, EventArgs e) => FinishedLoading();
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

        private void FinishedLoading()
        {

        }
    }
}
