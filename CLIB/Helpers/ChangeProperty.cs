using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CLIB.Helpers;

public class ChangeProperty : INotifyPropertyChanged
{
    public void OnPropertyChange([CallerMemberName]string propertyname="")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyname));
    }
    public event PropertyChangedEventHandler? PropertyChanged;
}
