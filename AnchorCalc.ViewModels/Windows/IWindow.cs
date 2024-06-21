using System.ComponentModel;

namespace AnchorCalc.ViewModels.Windows;

public interface IWindow
{
    void Show();
    void Close();
    event CancelEventHandler Closing;
    event EventHandler Closed;
}