using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Todo_List;

public partial class DesktopView : Window
{
    public static Grid GridInstance { get; set; }
    
    public static ScrollViewer ScrollViewerInstance { get; set; }
    
    private static DesktopView?_instance;

    public static DesktopView Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new DesktopView();
                _instance.InitializeComponent();
                
                GridInstance = _instance.Grid;
                ScrollViewerInstance = _instance.ScrollViewer;
            }

            return _instance;
        }
    }
    
    private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs ev)
    {
        DragMove();
    }
    
    protected override void OnClosing(CancelEventArgs ev)
    {
        ev.Cancel = true;
        this.Hide();
    }
}