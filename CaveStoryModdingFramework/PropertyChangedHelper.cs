using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CaveStoryModdingFramework
{
    public abstract class PropertyChangedHelper : INotifyPropertyChanging, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public event PropertyChangingEventHandler PropertyChanging;

        protected void NotifyPropertyChanging(string propertyName)
        {
            PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(propertyName));
        }
        protected void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        protected void SetVal<T>(ref T variable, T value, [CallerMemberName] string name = "")
        {
            if (!EqualityComparer<T>.Default.Equals(variable, value))
            {
                NotifyPropertyChanging(name);
                variable = value;
                NotifyPropertyChanged(name);
            }
        }
    }
}
