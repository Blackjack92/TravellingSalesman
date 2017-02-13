using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TravellingSalesman.Utils
{
    public abstract class NotifyPropertyChangedBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        // Create the OnPropertyChanged method to raise the event
        // This could be replaced by using PostSharp
        protected void OnPropertyChanged([CallerMemberName]string name = "Default")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
