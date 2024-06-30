using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using Todo_List.Enums;

namespace Todo_List.Generic
{
    public class Log
    {
        private static LogLevel LogLevel { get; set; } = LogLevel.Info;

        private static string LogPath { get; set; } = "LogOutput.log";

        private static bool WriteLogIntoFile { get; set; } = false;

        private static int MaxLogCount { get; set; } = 180;

        private static RichTextBox? LogTextBox { get; set; }

        private static readonly object fileLock = new object();

        public static bool SetLogLevel(LogLevel logLevel)
        {
            LogLevel = logLevel;
            return true;
        }

        public static bool SetIsWrite(bool isWrite)
        {
            WriteLogIntoFile = isWrite;
            return true;
        }

        private static bool WriteLineIntoFile(string line)
        {
            if (string.IsNullOrEmpty(line))
            {
                Error("Line is null or empty. This should not happen.");
                return false;
            }

            lock (fileLock)
            {
                try
                {
                    if (!File.Exists(LogPath))
                    {
                        using (var fileStream = File.Create(LogPath))
                        {
                            // Ensure the file is closed immediately
                        }
                    }

                    File.AppendAllText(LogPath, line + Environment.NewLine);
                    return true;
                }
                catch (Exception ex)
                {
                    Error($"\n{ex.Message}\n{ex.StackTrace}");
                    return false;
                }
            }
        }

        public static void ClearLog()
        {
            if (LogTextBox != null)
            {
                if (LogTextBox.Dispatcher.CheckAccess())
                {
                    LogTextBox.Document.Blocks.Clear();
                }
                else
                {
                    LogTextBox.Dispatcher.Invoke(() => LogTextBox.Document.Blocks.Clear());
                }
            }
            
            try
            {
                File.WriteAllText(LogPath, string.Empty);
            }
            catch (Exception ex)
            {
                Error($"Failed to clear local log file: {ex.Message}");
            }
        }

        private static void LogAddLine(string message, SolidColorBrush color)
        {
            if (LogTextBox != null)
            {
                if (LogTextBox.Dispatcher.CheckAccess())
                {
                    AppendLogMessage(message, color);
                }
                else
                {
                    LogTextBox.Dispatcher.Invoke(() => AppendLogMessage(message, color));
                }
            }
        }

        private static void AppendLogMessage(string message, SolidColorBrush color)
        {
            Paragraph paragraph = new Paragraph(new Run(message))
            {
                Foreground = color
            };

            LogTextBox.Document.Blocks.Add(paragraph);
            LogTextBox.ScrollToEnd();
        }

        private static bool LogEntry(LogLevel logLevel, string line, bool showSrc)
        {
            if (LogLevel >= logLevel)
            {
                lock (fileLock)
                {
                    if (LogTextBox != null)
                    {
                        if (LogTextBox.Document.Blocks.Count > MaxLogCount)
                        {
                            ClearLog();
                        }

                        string logMessage = showSrc
                            ? $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{GetCallerName()}] [{Enum.GetName(typeof(LogLevel), logLevel)}]: {line}"
                            : $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{Enum.GetName(typeof(LogLevel), logLevel)}]: {line}";

                        if (string.IsNullOrEmpty(logMessage))
                        {
                            return false;
                        }

                        if (WriteLogIntoFile)
                        {
                            WriteLineIntoFile(logMessage);
                        }

                        LogAddLine(logMessage, Brushes.CornflowerBlue);

                        return true;
                    }
                }
            }

            return false;
        }

        public static void Info(string message, bool showSrc = true)
        {
            LogEntry(LogLevel.Info, message, showSrc);
        }

        public static void Error(string message, bool showSrc = true)
        {
            LogEntry(LogLevel.Error, message, showSrc);
        }

        public static void Debug(string message, bool showSrc = true)
        {
            LogEntry(LogLevel.Debug, message, showSrc);
        }

        public static void Fatal(string message, bool showSrc = true)
        {
            LogEntry(LogLevel.Fatal, message, showSrc);
        }

        public static void Trace(string message, bool showSrc = true)
        {
            LogEntry(LogLevel.Trace, message, showSrc);
        }

        public static void SetLogTarget(RichTextBox richTextBox)
        {
            LogTextBox = richTextBox;
        }

        private static string GetCallerName()
        {
            MethodBase currentMethod = MethodBase.GetCurrentMethod();
            int frameCount = 0;

            while (true)
            {
                frameCount++;

                StackFrame callerFrame = new StackFrame(frameCount);
                MethodBase callerMethod = callerFrame.GetMethod();

                if (callerMethod == null || callerMethod.DeclaringType == null)
                {
                    return "Unknown Caller";
                }

                if (callerMethod.DeclaringType.Name == currentMethod.DeclaringType.Name || callerMethod.DeclaringType.Namespace != "Todo_List")
                {
                    continue;
                }
                else
                {
                    if (callerMethod.Name == "MoveNext" || callerMethod.Name == ".ctor")
                    {
                        string cleanMethodName = Regex.Replace(callerMethod.DeclaringType.Name, @"^<(\w+)>.*$", "$1");

                        return cleanMethodName;
                    }

                    return callerMethod.Name;
                }
            }
        }
    }
}
