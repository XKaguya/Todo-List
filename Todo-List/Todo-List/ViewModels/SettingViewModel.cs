using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Input;
using System.Xml.Serialization;
using Todo_List.Class;
using Todo_List.Enums;
using Todo_List.Generic;

namespace Todo_List.ViewModels
{
    public class SettingsViewModel : ViewModelBase
    {
        private bool _saveLogToFile;
        private bool _isDebugMode;
        private bool _canDrag;
        private bool _autoUpdate;
        private string _selectedLogMode;
        private double _selectedOpacity = 0.8;
        private ObservableCollection<string> _logModes;
        private Settings _settings;

        private static readonly string FilePath = "Settings.xml";
        
        public Settings Settings
        {
            get { return _settings; }
            set
            {
                _settings = value;
                OnPropertyChanged(nameof(Settings));
            }
        }

        public bool SaveLogToFile
        {
            get { return _saveLogToFile; }
            set
            {
                if (_saveLogToFile != value)
                {
                    _saveLogToFile = value;

                    bool isWriteEnabled = Generic.Log.SetIsWrite(_saveLogToFile);
                    if (isWriteEnabled)
                    {
                        Generic.Log.Info( "Log will now write into file.");
                    }
                    
                    OnPropertyChanged(nameof(SaveLogToFile));
                }
            }
        }
        
        public double SelectedOpacity
        {
            get => _selectedOpacity;
            set
            {
                if (_selectedOpacity != value)
                {
                    _selectedOpacity = value;
                    DesktopView.Instance.Opacity = _selectedOpacity;
                    OnPropertyChanged(nameof(SelectedOpacity));
                }
            }
        }

        public bool IsDebugMode
        {
            get { return _isDebugMode; }
            set
            {
                if (_isDebugMode != value)
                {
                    _isDebugMode = value;
                    OnPropertyChanged(nameof(IsDebugMode));
                }
            }
        }
        
        public bool AutoUpdate
        {
            get { return _autoUpdate; }
            set
            {
                if (_autoUpdate != value)
                {
                    _autoUpdate = value;
                    OnPropertyChanged(nameof(AutoUpdate));
                }
            }
        }
        
        public bool CanDrag
        {
            get { return _canDrag; }
            set
            {
                if (_canDrag != value)
                {
                    _canDrag = value;

                    if (_canDrag)
                    {
                        Generic.Log.Info("Dragable.");
                        
                        DesktopView.ScrollViewerInstance.IsHitTestVisible = false;
                        DesktopView.GridInstance.IsHitTestVisible = false;
                    }
                    else
                    {
                        Generic.Log.Info("Not Dragable.");
                        
                        DesktopView.ScrollViewerInstance.IsHitTestVisible = true;
                        DesktopView.GridInstance.IsHitTestVisible = true;
                    }
                    
                    OnPropertyChanged(nameof(CanDrag));
                }
            }
        }

        public string SelectedLogMode
        {
            get { return _selectedLogMode; }
            set
            {
                if (_selectedLogMode != value)
                {
                    _selectedLogMode = value;
                    OnPropertyChanged(nameof(SelectedLogMode));
                
                    if (Enum.TryParse(typeof(LogLevel), SelectedLogMode, out var logLevel))
                    {
                        Generic.Log.SetLogLevel((LogLevel)logLevel);
                    }
                    else
                    {
                        Generic.Log.Error("Invalid log level selected.");
                    }
                }
            }
        }
        
        public ObservableCollection<string> LogModes
        {
            get { return _logModes; }
            set
            {
                if (_logModes != value)
                {
                    _logModes = value;
                    
                    if (Enum.TryParse(typeof(LogLevel), SelectedLogMode, out var logLevel))
                    {
                        Generic.Log.SetLogLevel((LogLevel)logLevel);
                    }
                    else
                    {
                        Generic.Log.Error("Invalid log level selected.");
                    }
                    
                    OnPropertyChanged(nameof(LogModes));
                }
            }
        }

        public ICommand SaveCommand { get; }

        public SettingsViewModel()
        {
            SaveCommand = new RelayCommand(SaveSettings);
            LogModes = new ObservableCollection<string>
            {
                LogLevel.Fatal.ToString(),
                LogLevel.Trace.ToString(),
                LogLevel.Error.ToString(),
                LogLevel.Info.ToString(),
                LogLevel.Debug.ToString(),
                LogLevel.All.ToString()
            };

            LoadSettings();
        }

        private void LoadSettings()
        {
            Generic.Log.Info("Trying load setting file...");
            
            if (File.Exists(FilePath))
            {
                var serializer = new XmlSerializer(typeof(Settings));
                using var reader = new StreamReader(FilePath);
                Settings = (Settings)serializer.Deserialize(reader);
            }
            else
            {
                Settings = new Settings();
            }
            
            SaveLogToFile = Settings.SaveLogIntoFile;
            SelectedOpacity = Settings.SelectedOpacity;
            AutoUpdate = Settings.AutoUpdate;
            IsDebugMode = Settings.IsDebugMode;
            CanDrag = Settings.CanDrag;
            SelectedLogMode = Settings.LogLevel.ToString();

            MainWindow.IsAutoUpdate = AutoUpdate;
            
            Generic.Log.Debug($"Loaded {Settings.SaveLogIntoFile}, {Settings.AutoUpdate}, {Settings.IsDebugMode}, {Settings.LogLevel.ToString()}");
        }

        private void SaveSettings()
        {
            try
            {
                Generic.Log.Info("Attempting to save settings...");
                
                Settings.SaveLogIntoFile = SaveLogToFile;
                Settings.SelectedOpacity = SelectedOpacity;
                Settings.AutoUpdate = AutoUpdate;
                Settings.IsDebugMode = IsDebugMode;
                Settings.CanDrag = CanDrag;
                Settings.LogLevel = Enum.TryParse<LogLevel>(SelectedLogMode, out var logLevel) ? logLevel : LogLevel.Info;

                var serializer = new XmlSerializer(typeof(Settings));
                using (var writer = new StreamWriter(FilePath))
                {
                    serializer.Serialize(writer, Settings);
                }
                
                Setting.Instance.Hide();

                Generic.Log.Info("Settings saved successfully.");
            }
            catch (Exception ex)
            {
                Generic.Log.Error($"Failed to save settings: {ex.Message}");
                throw;
            }
        }
    }
}