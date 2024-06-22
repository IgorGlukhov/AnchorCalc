using System.ComponentModel;

namespace AnchorCalc.ViewModels.Windows;

public interface IWindow
{
    void Show();
    void Close();
    bool Activate();
    event CancelEventHandler Closing;
    event EventHandler Closed;
}