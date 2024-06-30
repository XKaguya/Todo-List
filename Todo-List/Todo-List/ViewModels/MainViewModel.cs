using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml.Serialization;
using Todo_List.Class;
using Todo_List.Generic;

namespace Todo_List.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        public ObservableCollection<ToDo> ToDoItems { get; set; }
        
        private static readonly string FilePath = "ToDoItems.xml";

        private string _message;
        public string Message
        {
            get => _message;
            private set => SetProperty(ref _message, value);
        }
        
        private Brush _color;
        public Brush Color
        {
            get => _color;
            set => SetProperty(ref _color, value);
        }

        public ICommand AddCommand { get; set; }
        public ICommand DeleteCommand { get; set; }
        public ICommand ExportCommand { get; set; }

        public MainViewModel()
        {
            ToDoItems = new ObservableCollection<ToDo>();
            AddCommand = new RelayCommand(AddToDo);
            DeleteCommand = new RelayCommand(DeleteSelectedToDo);
            ExportCommand = new RelayCommand(ExportToXml);

            LoadToDoItems();
        }

        private void AddToDo()
        {
            Message = string.Empty;

            try
            {
                var newToDo = new ToDo { Name = "New Todo", Description = "Description" };
                newToDo.PropertyChanged += OnPropertyChanged!;
                ToDoItems.Add(newToDo);
                SaveToDoItems();

                Color = Brushes.LightSeaGreen;
                Message = "New ToDo added.";
            }
            catch (Exception ex)
            {
                Color = Brushes.Red;
                Message = ex.Message;
                
                Generic.Log.Error(ex.Message + ex.StackTrace);
            }
        }

        private void OnPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            SaveToDoItems();
        }
        
        private void DeleteSelectedToDo()
        {
            Message = string.Empty;

            try
            {
                if (ToDoItems.Count == 0)
                {
                    Color = Brushes.Red;
                    Message = "There's no Todo in the list.";
                    return;
                }

                var selectedItems = ToDoItems.Where(item => item.IsSelected).ToList();

                if (selectedItems.Count > 0)
                {
                    foreach (var item in selectedItems)
                    {
                        ToDoItems.Remove(item);
                    }

                    SaveToDoItems();

                    Color = Brushes.LightSeaGreen;
                    Message = "Todo deleted.";
                }
                else
                {
                    Color = Brushes.Red;
                    Message = "No todo has been selected.";
                }
            }
            catch (Exception ex)
            {
                Color = Brushes.Red;
                Message = ex.Message;
                
                Generic.Log.Error(ex.Message + ex.StackTrace);
            }
        }

        private void LoadToDoItems()
        {
            Message = string.Empty;

            try
            {
                var serializer = new XmlSerializer(typeof(ObservableCollection<ToDo>));

                if (File.Exists(FilePath))
                {
                    using var reader = new StreamReader(FilePath);

                    var items = (ObservableCollection<ToDo>)serializer.Deserialize(reader);
                    ToDoItems.Clear();

                    foreach (var item in items)
                    {
                        ToDoItems.Add(item);
                        item.PropertyChanged += OnPropertyChanged!;
                    }

                    return;
                }

                Generic.Log.Info("File not exist. Load failed.");
            }
            catch (Exception ex)
            {
                Color = Brushes.Red;
                Message = ex.Message;
            }
        }

        private void SaveToDoItems()
        {
            try
            {
                var serializer = new XmlSerializer(typeof(ObservableCollection<ToDo>));
                using var writer = new StreamWriter(FilePath);
                serializer.Serialize(writer, ToDoItems);
            }
            catch (Exception ex)
            {
                Color = Brushes.Red;
                Message = ex.Message;
                Generic.Log.Error(ex.Message + ex.StackTrace);
            }
        }

        private void ExportToXml()
        {
            Message = string.Empty;

            try
            {
                if (ToDoItems.Count == 0)
                {
                    Color = Brushes.Red;
                    Message = "There's no todo to export.";
                    return;
                }

                var serializer = new XmlSerializer(typeof(ObservableCollection<ToDo>));
                using var writer = new StreamWriter("ExportsTodo.xml");
                serializer.Serialize(writer, ToDoItems);

                Color = Brushes.LightSeaGreen;
                Message = "Todo exported to XML successfully.";
                Generic.Log.Info($"Exported {ToDoItems.Count} items.");
            }
            catch (Exception ex)
            {
                Color = Brushes.Red;
                Message = ex.Message;
                Generic.Log.Error(ex.Message + ex.StackTrace);
            }
        }
    }
}
