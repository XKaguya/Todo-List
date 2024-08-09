using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Todo_List.ExternFeature
{
    public class AutoUpdate
    {
        private static readonly string Author = "Xkaguya";
        private static readonly string Project = "Todo-List";
        private static readonly string ExeName = "Todo-List.exe";
        private static readonly string CurrentExePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ExeName);
        private static readonly string NewExePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Todo-List.exe");
        private static readonly CancellationTokenSource CancellationTokenSource = new CancellationTokenSource();

        public static void StartAutoUpdateTask()
        {
            Task.Run(async () => await AutoUpdateTask(CancellationTokenSource.Token));
        }

        private static async Task AutoUpdateTask(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                CheckAndUpdate();
                await Task.Delay(TimeSpan.FromHours(1), token);
            }
        }

        public static void CheckAndUpdate()
        {
            try
            {
                string commonUpdaterPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "CommonUpdater.exe");

                if (!File.Exists(commonUpdaterPath))
                {
                    Generic.Log.Info("There's no CommonUpdater in the folder. Failed to update.");
                    return;
                }
                
                string arguments = $"{Project} {ExeName} {Author} {MainWindow.Version} \"{CurrentExePath}\" \"{NewExePath}\"";
                
                var startInfo = new ProcessStartInfo
                {
                    FileName = commonUpdaterPath,
                    Arguments = arguments,
                    UseShellExecute = false,
                    RedirectStandardOutput = false,
                    RedirectStandardError = true
                };

                Generic.Log.Debug($"Starting CommonUpdater with arguments: {arguments}");
                using var process = Process.Start(startInfo);
                if (process == null)
                {
                    Generic.Log.Error("Failed to start CommonUpdater: Process.Start returned null.");
                    return;
                }
                
                string error = process.StandardError.ReadToEnd();
                process.WaitForExit();
                
                if (!string.IsNullOrEmpty(error))
                {
                    Generic.Log.Error($"CommonUpdater error: {error}");
                }
                    
                if (process.ExitCode != 0)
                {
                    Generic.Log.Error($"CommonUpdater exited with code {process.ExitCode}");
                }
                else
                {
                    Generic.Log.Debug("CommonUpdater started successfully.");
                }
            }
            catch (Exception ex)
            {
                Generic.Log.Error($"Failed to start CommonUpdater: {ex.Message}");
            }
        }
    }
}