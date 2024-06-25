using System.Windows.Media.Media3D;
using AnchorCalc.Infrastructure.Calc;
using AnchorCalc.ViewModels.Plotters;

namespace AnchorCalc.ViewModels.MainWindow;

public class MainWindowSurfacePlotViewModel : ViewModel, IMainWindowSurfacePlotViewModel
{
    private double _a;

    private int _anchorCount;

    private double _b;

    private double _d;

    private Point3D[,] _dataPoints = new Point3D[1, 1];
    private double _e1;

    private double _h;

    private int _multipleX;

    private int _multipleY;

    private int _multipleZ;

    private double _mx;

    private double _my;

    private double _n;

    private double _rb;

    private string _title = string.Empty;

    private double _tough;

    private int _triangulation;

    private string _xAxisLabel = string.Empty;
    private double[,] _xData2DArray = new double[1, 1];

    private string _yAxisLabel = string.Empty;
    private double[,] _yData2DArray = new double[1, 1];

    private string _zAxisLabel = string.Empty;
    private double[,] _zData2DArray = new double[1, 1];

    public MainWindowSurfacePlotViewModel()
    {
        Title = "Напряжения в бетоне основания";
        XAxisLabel = "X";
        YAxisLabel = "Y";
        ZAxisLabel = "Z";
        N = 10;
        Mx = 3;
        My = 10;
        Tough = 100;
        H = 100;
        D = 12;
        Rb = 17;
        A = 300;
        B = 300;
        Triangulation = 100;
        MultipleX = 1000;
        MultipleY = 1000;
        MultipleZ = 10;
        E1 = 0.0015;
        Zsx = new double[,] { { -110, -110, 110, 110 } };
        Zsy = new double[,] { { -110, 110, -110, 110 } };

        var calc = new Calc(N, Mx, My, Tough, H, D, Rb, E1, A, B, Triangulation,
            Zsx, Zsy, MultipleX, MultipleY, MultipleZ);
        ZData2DArray = calc.Zbz;
        XData2DArray = calc.Zbx;
        YData2DArray = calc.Zby;
        DataPoints = PlotFromArray.Plot(XData2DArray, YData2DArray, ZData2DArray);
    }

    public double[,] XData2DArray
    {
        get => _xData2DArray;
        set
        {
            _xData2DArray = value;
            OnPropertyChanged();
        }
    }

    public double[,] Zsx { get; }
    public double[,] Zsy { get; }

    public double[,] YData2DArray
    {
        get => _yData2DArray;
        set
        {
            _yData2DArray = value;
            OnPropertyChanged();
        }
    }

    public double[,] ZData2DArray
    {
        get => _zData2DArray;
        set
        {
            _zData2DArray = value;
            OnPropertyChanged();
        }
    }

    public double E1
    {
        get => _e1;
        set
        {
            _e1 = value;
            OnPropertyChanged();
        }
    }

    public Point3D[,] DataPoints
    {
        get => _dataPoints;
        set
        {
            _dataPoints = value;
            OnPropertyChanged();
        }
    }

    public string Title
    {
        get => _title;
        set
        {
            _title = value;
            OnPropertyChanged();
        }
    }

    public string XAxisLabel
    {
        get => _xAxisLabel;
        set
        {
            _xAxisLabel = value;
            OnPropertyChanged();
        }
    }

    public string YAxisLabel
    {
        get => _yAxisLabel;
        set
        {
            _yAxisLabel = value;
            OnPropertyChanged();
        }
    }

    public string ZAxisLabel
    {
        get => _zAxisLabel;
        set
        {
            _zAxisLabel = value;
            OnPropertyChanged();
        }
    }

    public bool ShowMiniCoordinates { get; } = true;

    public double N
    {
        get => _n;
        set
        {
            _n = value;
            OnPropertyChanged();
        }
    }

    public double Mx
    {
        get => _mx;
        set
        {
            _mx = value;
            OnPropertyChanged();
        }
    }

    public double My
    {
        get => _my;
        set
        {
            _my = value;
            OnPropertyChanged();
        }
    }

    public double Tough
    {
        get => _tough;
        set
        {
            _tough = value;
            OnPropertyChanged();
        }
    }

    public double H
    {
        get => _h;
        set
        {
            _h = value;
            OnPropertyChanged();
        }
    }

    public double D
    {
        get => _d;
        set
        {
            _d = value;
            OnPropertyChanged();
        }
    }

    public double Rb
    {
        get => _rb;
        set
        {
            _rb = value;
            OnPropertyChanged();
        }
    }

    public double A
    {
        get => _a;
        set
        {
            _a = value;
            OnPropertyChanged();
        }
    }

    public double B
    {
        get => _b;
        set
        {
            _b = value;
            OnPropertyChanged();
        }
    }

    public int Triangulation
    {
        get => _triangulation;
        set
        {
            _triangulation = value;
            OnPropertyChanged();
        }
    }

    public int MultipleX
    {
        get => _multipleX;
        set
        {
            _multipleX = value;
            OnPropertyChanged();
        }
    }

    public int MultipleY
    {
        get => _multipleY;
        set
        {
            _multipleY = value;
            OnPropertyChanged();
        }
    }

    public int MultipleZ
    {
        get => _multipleZ;
        set
        {
            _multipleZ = value;
            OnPropertyChanged();
        }
    }

    public int AnchorCount
    {
        get => _anchorCount;
        set
        {
            _anchorCount = value;
            OnPropertyChanged();
        }
    }

    public void Dispose()
    {
    }
}