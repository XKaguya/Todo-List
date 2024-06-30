using System;
using System.Diagnostics;
using System.Windows;

namespace Todo_List
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static MainWindow? MainWindowInstance { get; set; }

        protected override void OnStartup(StartupEventArgs ev)
        {
            try
            {
                base.OnStartup(ev);

                KillExistingInstances();

                MainWindowInstance = new MainWindow();
                MainWindowInstance.Show();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
      
        private void KillExistingInstances()
        {
            var currentProcess = Process.GetCurrentProcess();
            var processes = Process.GetProcessesByName(currentProcess.ProcessName);

            foreach (var process in processes)
            {
                if (process.Id != currentProcess.Id)
                {
                    try
                    {
                        process.Kill();
                        process.WaitForExit();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Failed to kill process {process.Id}: {ex.Message}");
                    }
                }
            }
        }
    }
}