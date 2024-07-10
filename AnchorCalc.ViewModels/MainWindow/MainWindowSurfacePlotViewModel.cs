﻿using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using AnchorCalc.Infrastructure.Calc;
using AnchorCalc.ViewModels.Commands;
using AnchorCalc.ViewModels.Plotters;

namespace AnchorCalc.ViewModels.MainWindow;

public class MainWindowSurfacePlotViewModel : ViewModel, IMainWindowSurfacePlotViewModel
{
    private readonly Command _addCoordinateContainersCommand;
    private readonly Command _enterPropertiesCommand;
    private readonly List<TextBox> _listTextBoxX = [];
    private readonly List<TextBox> _listTextBoxY = [];
    private double _a = 300;

    private int _anchorCount = 4;

    private double _b = 300;

    private double _d = 12;

    private Point3D[,] _dataPoints = new Point3D[1, 1];
    private double _e1 = 0.0015;

    private double _h = 100;

    private int _multipleX = 1000;

    private int _multipleY = 1000;

    private int _multipleZ = 10;

    private double _mx = 3;

    private double _my = 10;

    private double _n = 10;
    private StackPanel _numberCoordinatesStack = new();
    private StackPanel _numberAnchorStack = new();
    private StackPanel _anchorForceStack = new();

    private double _rb = 17;

    private string _title = "Напряжения в бетоне основания";

    private double _tough = 100;

    private int _triangulation = 100;

    private string _xAxisLabel = "X";
    private StackPanel _xCoordinatesStack = new();
    private double[,] _xData2DArray = new double[1, 1];
    private double[,] _forceDataArray = new double[1, 1];

    private string _yAxisLabel = "Y";
    private StackPanel _yCoordinatesStack = new();
    private double[,] _yData2DArray = new double[1, 1];

    private string _zAxisLabel = "Z";
    private double[,] _zData2DArray = new double[1, 1];
    private double[,] _zsx = { { -110, -110, 110, 110 } };
    private double[,] _zsy = { { -110, 110, -110, 110 } };

    public MainWindowSurfacePlotViewModel()
    {
        _enterPropertiesCommand = new Command(EnterProperties);
        _addCoordinateContainersCommand = new Command(AddAnchorCoordinateContainers);
        ModelChange();
    }

    public StackPanel NumberCoordinatesStack
    {
        get => _numberCoordinatesStack;
        private set
        {
            _numberCoordinatesStack = value;
            OnPropertyChanged();
        }
    }
    public StackPanel NumberAnchorStack
    {
        get => _numberAnchorStack;
        private set
        {
            _numberAnchorStack = value;
            OnPropertyChanged();
        }
    }
    public StackPanel AnchorForceStack
    {
        get => _anchorForceStack;
        private set
        {
            _anchorForceStack = value;
            OnPropertyChanged();
        }
    }
    public StackPanel XCoordinatesStack
    {
        get => _xCoordinatesStack;
        private set
        {
            _xCoordinatesStack = value;
            OnPropertyChanged();
        }
    }

    public StackPanel YCoordinatesStack
    {
        get => _yCoordinatesStack;
        private set
        {
            _yCoordinatesStack = value;
            OnPropertyChanged();
        }
    }

    public ICommand EnterPropertiesCommand => _enterPropertiesCommand;
    public ICommand AddCoordinateContainersCommand => _addCoordinateContainersCommand;

    public double[,] XData2DArray
    {
        get => _xData2DArray;
        private set
        {
            _xData2DArray = value;
            OnPropertyChanged();
        }
    }
    public double[,] ForceDataArray
    {
        get => _forceDataArray;
        private set
        {
            _forceDataArray = value;
            OnPropertyChanged();
        }
    }

    public double[,] Zsx
    {
        get => _zsx;
        set
        {
            _zsx = value;
            OnPropertyChanged();
        }
    }

    public double[,] Zsy
    {
        get => _zsy;
        set
        {
            _zsy = value;
            OnPropertyChanged();
        }
    }


    public double[,] YData2DArray
    {
        get => _yData2DArray;
        private set
        {
            _yData2DArray = value;
            OnPropertyChanged();
        }
    }

    public double[,] ZData2DArray
    {
        get => _zData2DArray;
        private set
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
        private set
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

    private void AddAnchorCoordinateContainers()
    {
        NumberCoordinatesStack = new StackPanel();
        XCoordinatesStack = new StackPanel();
        YCoordinatesStack = new StackPanel();
        NumberAnchorStack = new StackPanel();
        AnchorForceStack = new StackPanel();
        _listTextBoxX.Clear();
        _listTextBoxY.Clear();

        for (var i = 0; i < AnchorCount; i++)
        {
            var border = new Border
            {
                BorderThickness = new Thickness(1),
                BorderBrush = new SolidColorBrush(Colors.Black),
                Background = new SolidColorBrush(Colors.White)
            };
            var textBoxX = new TextBox
            {
                Height = 16,
                BorderThickness = new Thickness(0),
                Background = new SolidColorBrush(Colors.White)
            };
            border.Child = textBoxX;
            if (Zsx.GetLength(1) > i) textBoxX.Text = Zsx[0, i].ToString(CultureInfo.CurrentCulture);
            _listTextBoxX.Add(textBoxX);
            XCoordinatesStack.Children.Add(border);
        }

        for (var i = 0; i < AnchorCount; i++)
        {
            var border = new Border
            {
                BorderThickness = new Thickness(1),
                BorderBrush = new SolidColorBrush(Colors.Black),
                Background = new SolidColorBrush(Colors.White)
            };
            var textBoxY = new TextBox
            {
                Height = 16,
                BorderThickness = new Thickness(0),
                Background = new SolidColorBrush(Colors.White)
            };
            border.Child = textBoxY;
            if (Zsy.GetLength(1) > i) textBoxY.Text = Zsy[0, i].ToString(CultureInfo.CurrentCulture);
            _listTextBoxY.Add(textBoxY);
            YCoordinatesStack.Children.Add(border);
        }

        for (var i = 0; i < AnchorCount; i++)
        {
            var border = new Border
            {
                BorderThickness = new Thickness(1),
                BorderBrush = new SolidColorBrush(Colors.Black),
                Background = new SolidColorBrush(Colors.White)
            };
            var textBoxNumber = new TextBlock
            {
                Text = (i + 1).ToString(),
                Height = 16,
                HorizontalAlignment = HorizontalAlignment.Center,
                Background = new SolidColorBrush(Colors.White)
            };
            border.Child = textBoxNumber;
            NumberCoordinatesStack.Children.Add(border);
        }
        for (var i = 0; i < AnchorCount; i++)
        {
            var border = new Border
            {
                BorderThickness = new Thickness(1),
                BorderBrush = new SolidColorBrush(Colors.Black),
                Background = new SolidColorBrush(Colors.White)
            };
            var textBlock = new TextBlock
            {
                Height = 16,
                Background = new SolidColorBrush(Colors.White)
            };
            border.Child = textBlock;
            if (ForceDataArray.GetLength(1) > i) textBlock.Text = $"{ForceDataArray[0, i]:0.00}";
            AnchorForceStack.Children.Add(border);
        }

        for (var i = 0; i < AnchorCount; i++)
        {
            var border = new Border
            {
                BorderThickness = new Thickness(1),
                BorderBrush = new SolidColorBrush(Colors.Black),
                Background = new SolidColorBrush(Colors.White)
            };
            var textBoxNumber = new TextBlock
            {
                Text = (i + 1).ToString(),
                Height = 16,
                HorizontalAlignment = HorizontalAlignment.Center,
                Background = new SolidColorBrush(Colors.White)
            };
            border.Child = textBoxNumber;
            NumberAnchorStack.Children.Add(border);
        }
    }
    private void EnterProperties()
    {
        if (_listTextBoxX.Count > 0 && _listTextBoxY.Count > 0)
        {
            Zsx = new double[1, AnchorCount];
            Zsy = new double[1, AnchorCount];
            for (var i = 0; i < AnchorCount; i++)
            {
                Zsx[0, i] = double.Parse(_listTextBoxX[i].Text);
                Zsy[0, i] = double.Parse(_listTextBoxY[i].Text);
            }
        }
        else
        {
            throw new Exception();
        }

        ModelChange();
    }

    private void ModelChange()
    {
        var calc = new Calc(N, Mx, My, Tough, H, D, Rb, E1, A, B, Triangulation,
            Zsx, Zsy, MultipleX, MultipleY, MultipleZ);
        ZData2DArray = calc.Zbz;
        XData2DArray = calc.Zbx;
        YData2DArray = calc.Zby;
        ForceDataArray = calc.Nan;
        DataPoints = PlotFromArray.Plot(XData2DArray, YData2DArray, ZData2DArray);
        AddAnchorCoordinateContainers();
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}