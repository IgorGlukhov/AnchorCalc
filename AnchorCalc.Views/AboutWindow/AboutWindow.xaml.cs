using System.Windows;
using AnchorCalc.ViewModels.AboutWindow;

namespace AnchorCalc.Views.AboutWindow;

public partial class AboutWindow : IAboutWindow
{
    public AboutWindow(IAboutWindowViewModel aboutWindowViewModel)
    {
        InitializeComponent();
        DataContext = aboutWindowViewModel;
    }
}