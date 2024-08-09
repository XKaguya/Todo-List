using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
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

        private static bool LogEntry(LogLevel logLevel, string line, string callerName = "")
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

                        string logMessage = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{callerName}] [{Enum.GetName(typeof(LogLevel), logLevel)}]: {line}";

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

        public static void Info(string message, [CallerMemberName] string callerName = "")
        {
            LogEntry(LogLevel.Info, message, callerName);
        }

        public static void Error(string message, [CallerMemberName] string callerName = "")
        {
            LogEntry(LogLevel.Error, message, callerName);
        }

        public static void Debug(string message, [CallerMemberName] string callerName = "")
        {
            LogEntry(LogLevel.Debug, message, callerName);
        }

        public static void Fatal(string message, [CallerMemberName] string callerName = "")
        {
            LogEntry(LogLevel.Fatal, message, callerName);
        }

        public static void Trace(string message, [CallerMemberName] string callerName = "")
        {
            LogEntry(LogLevel.Trace, message, callerName);
        }

        public static void SetLogTarget(RichTextBox richTextBox)
        {
            LogTextBox = richTextBox;
        }
    }
}
