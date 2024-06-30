using System.ComponentModel;
using System.Windows;

namespace Todo_List;

public partial class Setting : Window
{
    private static Setting? _instance;

    public static Setting Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new Setting();
                _instance.InitializeComponent();
            }

            return _instance;
        }
    }
    
    protected override void OnClosing(CancelEventArgs ev)
    {
        ev.Cancel = true;
        this.Hide();
    }
}