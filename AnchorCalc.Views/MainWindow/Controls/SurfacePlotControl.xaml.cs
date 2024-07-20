using System.Windows.Controls;
using System.Windows.Input;

namespace AnchorCalc.Views.MainWindow.Controls;

public partial class SurfacePlotControl : UserControl
{
    public SurfacePlotControl()
    {
        InitializeComponent();
        Viewport.ZoomExtentsGesture = new KeyGesture(Key.Space);
    }
}