using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TravellingSalesman.Utils
{
    /// <summary>
    /// This is a base class for the INotifyPropertyChanged. 
    /// Normally PostSharp could be used to replace this code.
    /// But at the moment there are was a bug with the mscore lib.
    /// </summary>
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
