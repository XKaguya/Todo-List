using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Todo_List
{
    public class ViewModelBase : INotifyPropertyChanged  
    {  
        public event PropertyChangedEventHandler PropertyChanged;  

        protected void OnPropertyChanged(string propname)  
        {
            if (PropertyChanged != null)  
            {  
                PropertyChanged(this, new PropertyChangedEventArgs(propname));  
            }  
        } 
        
        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(field, value))
            {
                return false;
            }
            
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }  
}