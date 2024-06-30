using Todo_List.Enums;

namespace Todo_List.Class
{
    public class Settings
    {
        public bool SaveLogIntoFile { get; set; }
        
        public bool IsDebugMode { get; set; }
        
        public bool CanDrag { get; set; }
        
        public LogLevel LogLevel { get; set; }
    }
}