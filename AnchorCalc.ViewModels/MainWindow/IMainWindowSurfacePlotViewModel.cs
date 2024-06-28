using System.Windows.Media.Media3D;

namespace AnchorCalc.ViewModels.MainWindow;

public interface IMainWindowSurfacePlotViewModel : IDisposable
{
    public Point3D[,] DataPoints { get; }
    public string Title { get; set; }

    public string XAxisLabel { get; set; }

    public string YAxisLabel { get; set; }

    public string ZAxisLabel { get; set; }

    public bool ShowMiniCoordinates { get; }

    public double N { get; set; }

    public double Mx { get; set; }

    public double My { get; set; }

    public double Tough { get; set; }

    public double H { get; set; }

    public double D { get; set; }

    public double Rb { get; set; }

    public double A { get; set; }

    public double B { get; set; }

    public int Triangulation { get; set; }

    public int MultipleX { get; set; }

    public int MultipleY { get; set; }

    public int MultipleZ { get; set; }

    public int AnchorCount { get; set; }
}