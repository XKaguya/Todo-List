using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace Todo_List;

public partial class Log : Window
{
    private static Log? _instance;
    
    private RichTextBox? logTextBox;

    public static Log Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new Log();
                _instance.InitializeComponent();
                _instance.logTextBox = _instance.LogTextBox;
                Generic.Log.SetLogTarget(_instance.logTextBox);
            }

            return _instance;
        }
    }
    
    private void ScrollToBottomClick(object sender, RoutedEventArgs ev)
    {
        LogTextBox.ScrollToEnd();
    }
    
    private void ClearLogClick(object sender, RoutedEventArgs ev)
    {
        Generic.Log.ClearLog();
    }
    
    protected override void OnClosing(CancelEventArgs ev)
    {
        ev.Cancel = true;
        this.Hide();
    }
}