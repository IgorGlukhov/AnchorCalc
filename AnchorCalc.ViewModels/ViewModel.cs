using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AnchorCalc.ViewModels;

public abstract class ViewModel:INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}