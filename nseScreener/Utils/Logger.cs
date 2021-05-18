using System;
using System.IO;

namespace Helper
{
    public enum LogLevel
    {
        DEBUG2 = 0,
        DEBUG,
        INFO,
        WARN,
        Database,
        ERROR,
        EXCEPTION,
        Fatal
    }
    public class Logger
    {
        private static readonly object fileLock = new object();
        private static readonly object consoleLock = new object();
        private static Logger log = new Logger();
        private LogLevel logLevel;
        private string Filename = null;
        private bool writeToConsole = true;


        private string TimeNow()
        {
            return DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");
        }

        public static ref Logger GetLoggerInstance() => ref log;
        public void LogToCosole(bool write) => writeToConsole = write;
        public void LogToFile(string file) {
            // Create the file if it did not exist
            if(!File.Exists(file))
            {
                try {
                    using(var fileStream = File.Create(file))
                    {
                        Filename = file;
                    }
                }
                catch(Exception ex)
                {
                    Error($"Unable to create log file '{file}' due to '{ex.Message}'.\n>>>>>>>Exiting with code 1<<<<<<<");
                    Environment.Exit(1);
                }
            }
            Filename = file;
            Info($"Logging to file {file}");
        }

        public void SetLogLevel(LogLevel level) {
            Console.WriteLine($"Setting log level to {level}");
            logLevel = level;
        }
        public void Debug2(string text) => Write($"{TimeNow()} DEBUG> {text}", LogLevel.DEBUG2);
        public void Debug(string text) => Write($"{TimeNow()} DEBUG> {text}", LogLevel.DEBUG);
        public void Info(string text) => Write($"{TimeNow()} INFO> {text}", LogLevel.INFO);
        public void Warn(string text) => Write($"{TimeNow()} WARN> {text}", LogLevel.WARN);
        public void Error(string text) => Write($"{TimeNow()} ERROR> {text}", LogLevel.ERROR);
        public void Database(string text) => Write($"{TimeNow()} DB> {text}", LogLevel.Database);
        public void Fatal(string text) 
        {
            Write($"{TimeNow()} Fatal> {text}", LogLevel.Fatal);
            Write($"****** Exiting program ********", LogLevel.Fatal);
            Environment.Exit(2);
        }

        public void printException(Exception ex) {
            Write($"Exception: {ex.Message}", LogLevel.EXCEPTION);
            Write("===============BackTrace====================", LogLevel.EXCEPTION);
            Write($"{ex.StackTrace}", LogLevel.EXCEPTION);
            Write("===============EndTrace====================\n", LogLevel.EXCEPTION);
        }

        private Logger()
        {
             logLevel = LogLevel.DEBUG;
        }

        private void Write(string text, LogLevel level)
        {
            if(level >= logLevel)
            {
                if(writeToConsole)
                {
                    lock(consoleLock)
                    {
                        switch(level)
                        {
                            case LogLevel.ERROR:
                            case LogLevel.EXCEPTION:
                            case LogLevel.Fatal:
                                Console.ForegroundColor = ConsoleColor.Red;
                                break;
                            case LogLevel.WARN:
                                Console.ForegroundColor = ConsoleColor.DarkRed;
                                break;
                            case LogLevel.INFO:
                                Console.ForegroundColor = ConsoleColor.Blue;
                                break;
                        }
                        if(level >= LogLevel.ERROR)  
                        { 
                            Console.ForegroundColor = ConsoleColor.Red;
                        }                         
                        Console.WriteLine("{0}", text);
                        Console.ResetColor();
                    }
                }
                if(Filename != null)
                {
                    lock(fileLock)
                    {
                        File.AppendAllText(Filename, $"{text}\n");
                    }
                }
            }
        }
    }
}