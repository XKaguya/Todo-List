using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Todo_List.Class;
using Todo_List.Enums;
using Todo_List.ExternFeature;
using Todo_List.Generic;
using Todo_List.ViewModels;

namespace Todo_List
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Point _dragStartPoint;
        
        private static object DataContextInstance { get; set; }

        private static string Version { get; } = "1.0.0";
        
        public MainWindow()
        {
            InitializeComponent();
            
            DataContextInstance = this.DataContext;
            
            Init();
        }

        private static void Init()
        {
            Log.Instance.Hide();
            
            Generic.Log.ClearLog();
            Generic.Log.Info($"Program started. Version {Version}.");
            
            DesktopView.Instance.Hide();
            DesktopView.Instance.DataContext = DataContextInstance;
            Setting.Instance.Hide();
        }
        
        private void SelectionPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs ev)
        {
            _dragStartPoint = ev.GetPosition(null);
        }
        
        protected override void OnClosing(CancelEventArgs ev)
        {
            Environment.Exit(0);
        }

        private void SelectionDrop(object sender, DragEventArgs ev)
        {
            if (ev.Data.GetDataPresent(typeof(ToDo)))
            {
                ToDo droppedItem = ev.Data.GetData(typeof(ToDo)) as ToDo;
                ListBox listBox = sender as ListBox;
                MainViewModel viewModel = DataContext as MainViewModel;
                if (viewModel != null && droppedItem != null && !viewModel.ToDoItems.Contains(droppedItem))
                {
                    viewModel.ToDoItems.Add(droppedItem);
                }
            }
        }

        private void SelectionChanged(object sender, SelectionChangedEventArgs ev)
        {
            if (ev.AddedItems.Count > 0)
            {
                foreach (var item in ev.AddedItems)
                {
                    if (item is ToDo toDoItem)
                    {
                        toDoItem.IsSelected = true;
                    }
                }
            }

            if (ev.RemovedItems.Count > 0)
            {
                foreach (var item in ev.RemovedItems)
                {
                    if (item is ToDo toDoItem)
                    {
                        toDoItem.IsSelected = false;
                    }
                }
            }
        }

        private void LogClick(object sender, RoutedEventArgs ev)
        {
            if (Log.Instance.IsVisible)
            {
                Log.Instance.Hide();
            }
            else
            {
                Log.Instance.Show();
            }
        }
        
        private void SettingClick(object sender, RoutedEventArgs ev)
        {
            if (Setting.Instance.IsVisible)
            {
                Setting.Instance.Hide();
            }
            else
            {
                Setting.Instance.Show();
            }
        }
        
        private void DeskViewClick(object sender, RoutedEventArgs ev)
        {
            if (DesktopView.Instance.IsVisible)
            {
                DesktopView.Instance.Hide();
            }
            else
            {
                DesktopView.Instance.Show();
            }
        }
    }
}