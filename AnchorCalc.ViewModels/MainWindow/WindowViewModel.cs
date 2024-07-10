using AnchorCalc.Domain.Settings;
using AnchorCalc.ViewModels.Windows;

namespace AnchorCalc.ViewModels.MainWindow;

public abstract class WindowViewModel<TWindowMementoWrapper>(TWindowMementoWrapper windowMementoWrapper)
    : ViewModel, IWindowViewModel
    where TWindowMementoWrapper : class, IWindowMementoWrapper
{
    public double Left
    {
        get => windowMementoWrapper.Left;
        set => windowMementoWrapper.Left = value;
    }

    public double Top
    {
        get => windowMementoWrapper.Top;
        set => windowMementoWrapper.Top = value;
    }

    public double Width
    {
        get => windowMementoWrapper.Width;
        set => windowMementoWrapper.Width = value;
    }

    public double Height
    {
        get => windowMementoWrapper.Height;
        set => windowMementoWrapper.Height = value;
    }

    public bool IsMaximized
    {
        get => windowMementoWrapper.IsMaximized;
        set => windowMementoWrapper.IsMaximized = value;
    }

    public virtual void WindowClosing()
    {
    }
}