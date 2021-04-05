using System;

namespace Media_Player
{
    public class Logger
    {
        #region Properties
        public static Logger Instance
        {
            get
            {
                if (instance == null)
                    instance = new Logger();

                return instance;
            }
        }
        private static Logger instance;
        #endregion


        public enum LogType
        {
            Console,
            MessageBox
        }


        public static void Log(bool comparison, LogType logType = LogType.Console) => Log(comparison.ToString(), logType);
        public static void Log(double number, LogType logType = LogType.Console) => Log(number.ToString(), logType);
        public static void Log(float number, LogType logType = LogType.Console) => Log(number.ToString(), logType);
        public static void Log(int number, LogType logType = LogType.Console) => Log(number.ToString(), logType);
        public static void Log(string text, LogType logType = LogType.Console)
        {
            LogEvents args = new LogEvents();
            args.Message = text;
            args.LogType = logType;
            Instance.OnMessageLogged(args);
        }


        #region Events
        public static event EventHandler<LogEvents> MessageLogged;

        public class LogEvents : EventArgs
        {
            public string Message { get; set; }
            public LogType LogType { get; set; }
        }

        /// <summary>
        /// When a message has been sent to the Output() function
        /// </summary>
        /// <param name="e">LogEvent args containing the output message</param>
        public void OnMessageLogged(LogEvents e)
        {
            EventHandler<LogEvents> handler = MessageLogged;
            if (handler != null)
                handler(this, e);
        }

        #endregion
    }
}
